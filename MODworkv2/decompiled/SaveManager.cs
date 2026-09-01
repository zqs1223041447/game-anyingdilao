using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Cysharp.Threading.Tasks;
using Data.SaveData;
using Data.SaveData.GlobalSave;
using Dialog;
using FinkFramework.Runtime.Data;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : Singleton<SaveManager>
{
	private class SaveSnapshot
	{
		public int SlotId;

		public SaveBackupKind BackupKind;

		public string SaveRootPath;

		public string SlotPath;

		public string GlobalSavePath;

		public string RecoveryMetaPath;

		public string RecoveryMetaBackupPath;

		public string ManifestPath;

		public string ManifestBackupPath;

		public SaveData SlotData;

		public GlobalSaveData GlobalData;

		public RecoveryMeta Meta;
	}

	private class SaveTaskState
	{
		public SaveSnapshot Snapshot;

		public Task DiskTask;

		public bool CompletionHandled;
	}

	private class SaveManifest
	{
		public int Version = 3;

		public long UpdatedUtcTicks;

		public string GlobalPath = "";

		public long GlobalTicks;

		public string GlobalTransactionId = "";

		public List<SaveManifestSlotEntry> Slots = new List<SaveManifestSlotEntry>();

		public void Normalize()
		{
			Version = 3;
			GlobalPath = GlobalPath ?? "";
			GlobalTransactionId = GlobalTransactionId ?? "";
			Slots = Slots ?? new List<SaveManifestSlotEntry>();
			for (int num = Slots.Count - 1; num >= 0; num--)
			{
				if (Slots[num] == null || Slots[num].SlotId < 0)
				{
					Slots.RemoveAt(num);
				}
				else
				{
					Slots[num].Normalize();
				}
			}
			Slots = (from s in Slots
				group s by s.SlotId into g
				select g.OrderByDescending((SaveManifestSlotEntry s) => s.UpdatedUtcTicks).First() into s
				orderby s.SlotId
				select s).ToList();
		}

		public SaveManifestSlotEntry GetOrCreateSlot(int slotId)
		{
			Normalize();
			SaveManifestSlotEntry saveManifestSlotEntry = Slots.FirstOrDefault((SaveManifestSlotEntry s) => s.SlotId == slotId);
			if (saveManifestSlotEntry != null)
			{
				return saveManifestSlotEntry;
			}
			saveManifestSlotEntry = new SaveManifestSlotEntry
			{
				SlotId = slotId
			};
			Slots.Add(saveManifestSlotEntry);
			return saveManifestSlotEntry;
		}
	}

	private class SaveManifestSlotEntry
	{
		public int SlotId;

		public string BaselinePath = "";

		public string AutoPath = "";

		public string ExitPath = "";

		public long BaselineTicks;

		public long AutoTicks;

		public long ExitTicks;

		public string SessionId = "";

		public long UpdatedUtcTicks;

		public string UiGameVersion = "";

		public long UiPlayTimeSeconds;

		public int UiPlayerType;

		public string UiPlayerName = "";

		public int UiLevel;

		public int UiDFLevel;

		public void Normalize()
		{
			BaselinePath = BaselinePath ?? "";
			AutoPath = AutoPath ?? "";
			ExitPath = ExitPath ?? "";
			SessionId = (IsValidSessionId(SessionId) ? NormalizeSessionId(SessionId) : "");
			UiGameVersion = UiGameVersion ?? "";
			UiPlayerName = UiPlayerName ?? "";
			if (BaselineTicks < 0)
			{
				BaselineTicks = 0L;
			}
			if (AutoTicks < 0)
			{
				AutoTicks = 0L;
			}
			if (ExitTicks < 0)
			{
				ExitTicks = 0L;
			}
			if (UpdatedUtcTicks < 0)
			{
				UpdatedUtcTicks = 0L;
			}
			if (UiPlayTimeSeconds < 0)
			{
				UiPlayTimeSeconds = 0L;
			}
			if (UiLevel < 0)
			{
				UiLevel = 0;
			}
			if (UiDFLevel < 0)
			{
				UiDFLevel = 0;
			}
		}

		public bool HasUiSummary()
		{
			if (!string.IsNullOrEmpty(UiPlayerName) && UiLevel > 0)
			{
				return UiDFLevel > 0;
			}
			return false;
		}

		public string GetPath(SaveBackupKind backupKind)
		{
			return backupKind switch
			{
				SaveBackupKind.AutoBackup => AutoPath, 
				SaveBackupKind.ExitBackup => ExitPath, 
				_ => BaselinePath, 
			};
		}
	}

	private class RecoveryMeta
	{
		public int Version;

		public int SlotId;

		public long BaselineUtcTicks;

		public string SessionId;

		public long LastAutoBackupUtcTicks;

		public long LastExitBackupUtcTicks;

		public void Normalize(int slotId, SaveData baselineData)
		{
			Version = 1;
			SlotId = slotId;
			long num = GetBaselineTicksFromData(baselineData);
			if (num <= 0)
			{
				num = DateTime.UtcNow.Ticks;
			}
			if (BaselineUtcTicks <= 0)
			{
				BaselineUtcTicks = num;
			}
			if (string.IsNullOrEmpty(SessionId))
			{
				SessionId = ((!string.IsNullOrEmpty(baselineData?.SessionId)) ? baselineData.SessionId : Guid.NewGuid().ToString("N"));
			}
			if (!IsValidSessionId(SessionId))
			{
				SessionId = Guid.NewGuid().ToString("N");
			}
			else
			{
				SessionId = NormalizeSessionId(SessionId);
			}
			if (LastAutoBackupUtcTicks < 0)
			{
				LastAutoBackupUtcTicks = 0L;
			}
			if (LastExitBackupUtcTicks < 0)
			{
				LastExitBackupUtcTicks = 0L;
			}
		}
	}

	private const string LegacySaveFolder = "Saves";

	private const string AutoBackupSuffix = "_auto";

	private const string ExitBackupSuffix = "_exit";

	private const string RecoveryMetaSuffix = "_recovery.meta";

	private const string RecoveryMetaBackupSuffix = "_recovery.meta.bak";

	private const string ManifestFileName = "save_manifest.meta";

	private const string GlobalDeletedTransactionId = "__global_deleted__";

	private const int ReplaceBackupKeepCount = 3;

	private const int ManifestVersion = 3;

	private const int ManifestMagic = 1296454477;

	private const int ManifestCheckKey = 1511506142;

	private const int RecoveryMetaVersion = 1;

	private const int RecoveryMetaCheckKey = 1248819489;

	private static readonly string[] KnownSaveExtensions = new string[2] { ".sav", ".json" };

	private const string SaveExt = ".sav";

	private static bool isSaving;

	private static bool lastSaveSucceeded;

	private static Task currentSaveTask;

	private static SaveTaskState currentSaveState;

	private static readonly object saveStateLock = new object();

	private static readonly byte[] LastSlotHeader = new byte[4] { 70, 73, 78, 75 };

	private const int LastSlotCheckKey = 324478056;

	private const int LastSlotFileLength = 12;

	private const string GlobalFileName = "global";

	public static SaveData RuntimeData { get; private set; }

	public static bool HasRuntime => RuntimeData != null;

	public static int CurrentSlotId { get; private set; } = -1;


	private static string SaveRoot => Application.persistentDataPath;

	private static string LegacySaveRoot => Path.Combine(Application.persistentDataPath, "Saves");

	public static bool IsSaving => isSaving;

	private static string LastSlotPath => Path.Combine(SaveRoot, "last_save_id.sav");

	public static GlobalSaveData RuntimeGlobalData { get; private set; }

	public static bool HasGlobalRuntime => RuntimeGlobalData != null;

	private static string GlobalPath => Path.Combine(SaveRoot, "global.sav");

	private static string ManifestPath => GetManifestPath(SaveRoot);

	private static string ManifestBackupPath => GetManifestBackupPath(SaveRoot);

	public static void SaveLastSlot(int slotId)
	{
		if (slotId < 0 || !HasSaveSlot(slotId))
		{
			LogUtil.Warn($"SaveLastSlot: 非法或不存在的槽位 Id = {slotId}，已改为清空最近记录。");
			SafeDeleteLastSlotFile();
			return;
		}
		try
		{
			EnsureFolder();
			string text = LastSlotPath + ".tmp";
			if (File.Exists(text))
			{
				File.Delete(text);
			}
			WriteLastSlotFile(text, slotId);
			ReplaceFileKeepingBackup(text, LastSlotPath, LastSlotPath + ".bak");
		}
		catch (Exception arg)
		{
			LogUtil.Error($"保存最近游玩槽位失败：{arg}");
		}
	}

	public static int GetLastSlot()
	{
		try
		{
			foreach (string item in GetRecoverablePaths(LastSlotPath).OrderByDescending(SafeGetLastWriteTimeUtc))
			{
				if (TryReadLastSlotFile(item, out var slotId))
				{
					return slotId;
				}
			}
			return -1;
		}
		catch (Exception ex)
		{
			LogUtil.Warn("读取最近游玩槽位失败。原因：" + ex.Message);
			return -1;
		}
	}

	public static void ClearLastSlot()
	{
		SafeDeleteLastSlotFile();
	}

	private static bool IsLastSlotHeaderValid(byte[] header)
	{
		if (header == null || header.Length != LastSlotHeader.Length)
		{
			return false;
		}
		for (int i = 0; i < LastSlotHeader.Length; i++)
		{
			if (header[i] != LastSlotHeader[i])
			{
				return false;
			}
		}
		return true;
	}

	private static void SafeDeleteLastSlotFile()
	{
		try
		{
			DeleteRecoverableFiles(LastSlotPath);
		}
		catch (Exception ex)
		{
			LogUtil.Warn("删除最近游玩槽位文件失败：" + ex.Message);
		}
	}

	private static void WriteLastSlotFile(string path, int slotId)
	{
		using FileStream fileStream = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None);
		using BinaryWriter binaryWriter = new BinaryWriter(fileStream);
		binaryWriter.Write(LastSlotHeader);
		binaryWriter.Write(slotId);
		binaryWriter.Write(slotId ^ 0x13572468);
		binaryWriter.Flush();
		fileStream.Flush();
	}

	private static bool TryReadLastSlotFile(string path, out int slotId)
	{
		slotId = -1;
		try
		{
			using FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
			using BinaryReader binaryReader = new BinaryReader(fileStream);
			if (fileStream.Length != 12)
			{
				return false;
			}
			if (!IsLastSlotHeaderValid(binaryReader.ReadBytes(LastSlotHeader.Length)))
			{
				return false;
			}
			int num = binaryReader.ReadInt32();
			int num2 = binaryReader.ReadInt32();
			if ((num ^ 0x13572468) != num2)
			{
				return false;
			}
			if (num < 0 || !HasSaveSlot(num))
			{
				return false;
			}
			slotId = num;
			return true;
		}
		catch (Exception ex)
		{
			LogUtil.Warn("读取最近游玩槽位候选文件失败：" + path + ", reason = " + ex.Message);
			return false;
		}
	}

	public static async UniTask<bool> SaveAndWaitIfNeeded()
	{
		if (IsSaving)
		{
			while (IsSaving)
			{
				await UniTask.Yield(PlayerLoopTiming.Update);
			}
			return lastSaveSucceeded;
		}
		if (!RequestSave())
		{
			return false;
		}
		while (IsSaving)
		{
			await UniTask.Yield(PlayerLoopTiming.Update);
		}
		return lastSaveSucceeded;
	}

	public static async UniTask<bool> SaveAndExitAndWaitIfNeeded()
	{
		await WaitForCurrentSaveAsync();
		if (!RequestSaveAndExit())
		{
			return false;
		}
		while (IsSaving)
		{
			await UniTask.Yield(PlayerLoopTiming.Update);
		}
		return lastSaveSucceeded;
	}

	public static bool RequestSave()
	{
		if (isSaving)
		{
			return false;
		}
		return SaveCurrentGameAsync();
	}

	public static bool SaveAndExitBlocking()
	{
		WaitForCurrentSaveBlocking();
		return SaveCurrentGameBlocking(SaveBackupKind.ExitBackup);
	}

	public static bool RequestSaveAndExit()
	{
		if (isSaving)
		{
			QueueExitSaveAfterCurrentSave();
			return true;
		}
		return SaveCurrentGameAsync(SaveBackupKind.ExitBackup);
	}

	public static bool SaveCurrentGameAsync(SaveBackupKind backupKind = SaveBackupKind.AutoBackup)
	{
		if (!IsRuntimeSaveBackupKind(backupKind))
		{
			LogUtil.Error($"非法运行时保存类型：{backupKind}。运行时只允许写入自动备份或保存并退出备份，禁止覆盖进入基线。");
			return false;
		}
		if (CurrentSlotId < 0)
		{
			return false;
		}
		if (SceneManager.GetActiveScene().name == "StartScene")
		{
			return false;
		}
		if (!HasRuntime)
		{
			LogUtil.Error("SaveRuntime 未初始化，无法保存");
			return false;
		}
		if (isSaving)
		{
			LogUtil.Warn("当前已有存档任务进行中，本次保存跳过。");
			return false;
		}
		SaveSnapshot snapshot = BuildSaveSnapshot(backupKind);
		if (snapshot == null)
		{
			LogUtil.Error("构建存档快照失败，无法执行异步保存。");
			return false;
		}
		isSaving = true;
		lastSaveSucceeded = false;
		currentSaveTask = CompleteSaveAfterDiskWriteAsync(currentSaveState = new SaveTaskState
		{
			Snapshot = snapshot,
			DiskTask = Task.Run(delegate
			{
				WriteSnapshotToDisk(snapshot);
			})
		});
		_ = currentSaveTask;
		return true;
	}

	public static bool EnterGameWithSlot(int slotId)
	{
		SetCurrentSlot(slotId);
		if (CurrentSlotId == -1)
		{
			LogUtil.Error("未正确设置当前槽位Id,无法加载存档数据");
			return false;
		}
		if (!TryResolveSlotForEntry(CurrentSlotId, out var data, out var loadedPath))
		{
			LogUtil.Error($"存档 slot_{CurrentSlotId} 加载失败，未找到可恢复备份。");
			ResetCurrentSlotIndex();
			return false;
		}
		LoadGlobalData();
		ReconcileGlobalDataWithSlot(data);
		if (!BeginNewSession(CurrentSlotId, data))
		{
			ResetCurrentSlotIndex();
			ResetGlobalData();
			LogUtil.Error($"slot_{slotId} 进入游戏基线写入失败，已取消进入游戏，避免后续自动存档生成不可恢复的会话。");
			return false;
		}
		LogUtil.Info($"已为 slot_{CurrentSlotId} 选择进入存档：{Path.GetFileName(loadedPath)}");
		BuildRuntimeFromSaveData(data);
		return true;
	}

	public static void ResetSaveData()
	{
		ResetCurrentSlotIndex();
		RuntimeData = null;
		ResetGlobalData();
	}

	public static bool TryCreateNewSlot(string playerName, int typeId, out int newSlotId)
	{
		newSlotId = -1;
		try
		{
			SaveData data = CreateInitData(playerName, typeId);
			int num = AllocateNewSlotId();
			if (!SaveToSlot(num, data))
			{
				LogUtil.Error($"创建新存档失败：slot_{num}");
				return false;
			}
			UpdateManifestForBaseline(num, data, GetSlotPath(num));
			newSlotId = num;
			return true;
		}
		catch (Exception arg)
		{
			LogUtil.Error($"创建新存档异常：{arg}");
			return false;
		}
	}

	public static bool DeleteSaveSlot(int slotId)
	{
		if (isSaving)
		{
			LogUtil.Warn($"当前正在保存，已拒绝删除 slot_{slotId}，避免删除与写盘并发。");
			return false;
		}
		if (slotId == 0)
		{
			LogUtil.Warn("自动存档槽位 (slot_0) 不允许删除。");
			return false;
		}
		if (!HasSaveSlot(slotId))
		{
			LogUtil.Warn($"尝试删除不存在的存档槽位：{slotId}");
			return false;
		}
		try
		{
			bool flag = GetLastSlot() == slotId;
			foreach (string slotBackupPathVariant in GetSlotBackupPathVariants(slotId, SaveBackupKind.EntryBaseline))
			{
				DeleteRecoverableFiles(slotBackupPathVariant);
			}
			foreach (string slotBackupPathVariant2 in GetSlotBackupPathVariants(slotId, SaveBackupKind.AutoBackup))
			{
				DeleteRecoverableFiles(slotBackupPathVariant2);
			}
			foreach (string slotBackupPathVariant3 in GetSlotBackupPathVariants(slotId, SaveBackupKind.ExitBackup))
			{
				DeleteRecoverableFiles(slotBackupPathVariant3);
			}
			DeleteRecoverableFiles(GetRecoveryMetaPath(slotId));
			DeleteRecoverableFiles(GetRecoveryMetaBackupPath(slotId));
			if (flag)
			{
				ClearLastSlot();
				LogUtil.Info($"最近游玩存档 slot_{slotId} 已被删除，已清空最近记录。");
			}
			if (CurrentSlotId == slotId)
			{
				ResetCurrentSlotIndex();
			}
			RemoveSlotFromManifest(slotId);
			LogUtil.Info($"存档槽位 {slotId} 已删除。");
			return true;
		}
		catch (Exception arg)
		{
			LogUtil.Error($"删除存档槽位 {slotId} 失败：{arg}");
			return false;
		}
	}

	public static IEnumerable<SaveSlotData> GetAllSaveSlotForUI()
	{
		SaveManifest saveManifest = LoadSaveManifest();
		HashSet<int> yielded = new HashSet<int>();
		foreach (SaveManifestSlotEntry slot in saveManifest.Slots)
		{
			if (slot == null || slot.SlotId < 0 || !yielded.Add(slot.SlotId))
			{
				continue;
			}
			if (TryBuildSaveSlotDataFromManifest(slot, out var data))
			{
				yield return data;
				continue;
			}
			int slotId = slot.SlotId;
			if (!TryPeekSlotForEntry(slotId, out var data2, out var loadedPath))
			{
				LogUtil.Warn($"存档 slot_{slotId} 无法读取任何可用备份，已跳过。");
				continue;
			}
			UpdateManifestUiSummaryBestEffort(slotId, data2, loadedPath);
			yield return BuildSaveSlotData(slotId, data2);
		}
		foreach (int allSlot in GetAllSlots())
		{
			if (yielded.Add(allSlot))
			{
				if (!TryPeekSlotForEntry(allSlot, out var data3, out var loadedPath2))
				{
					LogUtil.Warn($"存档 slot_{allSlot} 无法读取任何可用备份，已跳过。");
					continue;
				}
				UpdateManifestUiSummaryBestEffort(allSlot, data3, loadedPath2);
				yield return BuildSaveSlotData(allSlot, data3);
			}
		}
	}

	public static bool HasAnySaveSlot()
	{
		return GetAllSlots().Any(HasSaveSlot);
	}

	private static SaveSlotData BuildSaveSlotData(int slotId, SaveData data)
	{
		return new SaveSlotData
		{
			SlotId = slotId,
			GameVersion = data.GameVersion,
			PlayTimeSeconds = data.PlayTimeSeconds,
			PlayerType = data.PlayerData.PlayerType,
			playerName = data.PlayerData.PlayerName,
			level = data.PlayerData.Level,
			dfLevel = Mathf.Max(1, data.PlayerData.DFLevel)
		};
	}

	private static void UpdateEntryUiSummary(SaveManifestSlotEntry entry, SaveData data)
	{
		if (entry != null && data != null && data.PlayerData != null)
		{
			entry.UiGameVersion = data.GameVersion ?? "";
			entry.UiPlayTimeSeconds = data.PlayTimeSeconds;
			entry.UiPlayerType = data.PlayerData.PlayerType;
			entry.UiPlayerName = data.PlayerData.PlayerName ?? "";
			entry.UiLevel = data.PlayerData.Level;
			entry.UiDFLevel = Mathf.Max(1, data.PlayerData.DFLevel);
		}
	}

	private static void UpdateManifestUiSummaryBestEffort(int slotId, SaveData data, string loadedPath)
	{
		if (data == null)
		{
			return;
		}
		try
		{
			SaveManifest saveManifest = LoadSaveManifest();
			SaveManifestSlotEntry orCreateSlot = saveManifest.GetOrCreateSlot(slotId);
			string text = (string.IsNullOrEmpty(loadedPath) ? "" : ToManifestRelativePath(loadedPath, SaveRoot));
			switch (data.BackupKind)
			{
			case SaveBackupKind.AutoBackup:
				if (!string.IsNullOrEmpty(text))
				{
					orCreateSlot.AutoPath = text;
				}
				orCreateSlot.AutoTicks = data.SaveCreatedUtcTicks;
				break;
			case SaveBackupKind.ExitBackup:
				if (!string.IsNullOrEmpty(text))
				{
					orCreateSlot.ExitPath = text;
				}
				orCreateSlot.ExitTicks = data.SaveCreatedUtcTicks;
				break;
			default:
				if (!string.IsNullOrEmpty(text))
				{
					orCreateSlot.BaselinePath = text;
				}
				orCreateSlot.BaselineTicks = data.SaveCreatedUtcTicks;
				break;
			}
			orCreateSlot.SessionId = data.SessionId;
			orCreateSlot.UpdatedUtcTicks = DateTime.UtcNow.Ticks;
			UpdateEntryUiSummary(orCreateSlot, data);
			SaveManifestBestEffort(saveManifest, ManifestPath, ManifestBackupPath);
		}
		catch (Exception ex)
		{
			LogUtil.Warn($"补写存档 UI 摘要失败：slot_{slotId}, reason = {ex.Message}");
		}
	}

	private static bool TryBuildSaveSlotDataFromManifest(SaveManifestSlotEntry entry, out SaveSlotData data)
	{
		data = null;
		if (entry == null || !entry.HasUiSummary())
		{
			return false;
		}
		if (!HasRecoverableManifestEntryPath(entry))
		{
			return false;
		}
		data = new SaveSlotData
		{
			SlotId = entry.SlotId,
			GameVersion = entry.UiGameVersion,
			PlayTimeSeconds = entry.UiPlayTimeSeconds,
			PlayerType = entry.UiPlayerType,
			playerName = entry.UiPlayerName,
			level = entry.UiLevel,
			dfLevel = entry.UiDFLevel
		};
		return true;
	}

	public static void DeleteAllSaveData()
	{
		if (isSaving)
		{
			LogUtil.Warn("当前正在保存，已拒绝删除所有存档，避免删除与写盘并发。");
			return;
		}
		try
		{
			int num = 0;
			foreach (string item in EnumerateSaveFilesForDelete().ToList())
			{
				DeleteSlotFileIfExists(item);
				num++;
			}
			if (Directory.Exists(LegacySaveRoot))
			{
				Directory.Delete(LegacySaveRoot, recursive: true);
			}
			LogUtil.Info((num > 0) ? $"所有存档文件已删除：{num} 个。" : "未发现需要删除的存档文件。");
		}
		catch (Exception ex)
		{
			LogUtil.Error("删除所有存档失败: " + ex.Message);
		}
		ResetCurrentSlotIndex();
		RuntimeGlobalData = null;
		RuntimeData = null;
		lastSaveSucceeded = false;
	}

	public static void LoadGlobalData()
	{
		EnsureFolder();
		if (TryLoadValidGlobalData(GetGlobalPathVariants(), out var data, out var loadedPath))
		{
			long ticks = ((data.SaveCreatedUtcTicks > 0) ? data.SaveCreatedUtcTicks : SafeGetLastWriteTimeUtc(loadedPath).Ticks);
			if (!IsGlobalSuppressedByDeleteMarker(data, ticks))
			{
				RuntimeGlobalData = data;
				LogUtil.Info("已加载全局存档 global：" + Path.GetFileName(loadedPath) + "。");
				return;
			}
			LogUtil.Warn("检测到 global 删除标记比现有 global 更新，已忽略旧 global 文件。");
		}
		RuntimeGlobalData = GlobalSaveData.CreateNew();
		RuntimeGlobalData.PostLoadFix();
		LogUtil.Warn("未找到可用全局存档，已先使用内存默认 global，等待槽位内嵌副本修复或下次保存写回。");
	}

	public static void ResetGlobalData()
	{
		RuntimeGlobalData = null;
	}

	private static void ReconcileGlobalDataWithSlot(SaveData slotData)
	{
		if (slotData == null || slotData.EmbeddedGlobalData == null || string.IsNullOrEmpty(slotData.SaveTransactionId))
		{
			return;
		}
		slotData.EmbeddedGlobalData.PostLoadFix();
		long num = ((slotData.EmbeddedGlobalData.SaveCreatedUtcTicks > 0) ? slotData.EmbeddedGlobalData.SaveCreatedUtcTicks : slotData.SaveCreatedUtcTicks);
		if (IsGlobalDeleteMarkerNewerThan(num))
		{
			return;
		}
		bool flag = RuntimeGlobalData == null || (RuntimeGlobalData.LastWriterSlotId < 0 && string.IsNullOrEmpty(RuntimeGlobalData.SaveTransactionId));
		bool flag2 = RuntimeGlobalData != null && RuntimeGlobalData.LastWriterSlotId == CurrentSlotId && RuntimeGlobalData.SaveTransactionId != slotData.SaveTransactionId;
		bool num2 = RuntimeGlobalData != null && RuntimeGlobalData.LastWriterSlotId == CurrentSlotId && RuntimeGlobalData.SaveTransactionId == slotData.SaveTransactionId;
		bool flag3 = RuntimeGlobalData != null && RuntimeGlobalData.LastWriterSlotId != CurrentSlotId && num > RuntimeGlobalData.SaveCreatedUtcTicks;
		if (num2 || (!flag && !flag2 && !flag3))
		{
			return;
		}
		RuntimeGlobalData = slotData.EmbeddedGlobalData;
		RuntimeGlobalData.LastWriterSlotId = CurrentSlotId;
		RuntimeGlobalData.SaveTransactionId = slotData.SaveTransactionId;
		RuntimeGlobalData.SaveCreatedUtcTicks = num;
		RuntimeGlobalData.PostLoadFix();
		try
		{
			AtomicSave(GlobalPath, RuntimeGlobalData);
			UpdateManifestForGlobal(RuntimeGlobalData, GlobalPath);
			LogUtil.Warn($"global 与 slot_{CurrentSlotId} 恢复候选不一致，已使用槽位内嵌 global 副本修复。");
		}
		catch (Exception arg)
		{
			LogUtil.Error($"global 事务修复写回失败，已仅使用内存中的槽位内嵌 global 副本：{arg}");
		}
	}

	public static bool DeleteGlobalData()
	{
		if (isSaving)
		{
			LogUtil.Warn("当前正在保存，已拒绝删除 global，避免删除与写盘并发。");
			return false;
		}
		try
		{
			bool flag = GetGlobalPathVariants().Any((string path) => GetRecoverablePaths(path).Any());
			if (!MarkGlobalDeletedInManifest())
			{
				LogUtil.Error("删除全局存档失败：global 删除标记写入失败，已停止删除实体文件以避免旧数据被误恢复。");
				return false;
			}
			foreach (string globalPathVariant in GetGlobalPathVariants())
			{
				DeleteRecoverableFiles(globalPathVariant);
			}
			LogUtil.Info(flag ? "全局存档 global 已删除。" : "全局存档 global 不存在，无需删除。");
			RuntimeGlobalData = null;
			return flag;
		}
		catch (Exception ex)
		{
			LogUtil.Error("删除全局存档失败: " + ex.Message);
			RuntimeGlobalData = null;
			return false;
		}
	}

	private static SaveSnapshot BuildSaveSnapshot(SaveBackupKind backupKind)
	{
		try
		{
			SaveData saveData = DataUtil.DeepClone(RuntimeData) ?? SaveData.CreateNew();
			string saveTransactionId = Guid.NewGuid().ToString("N");
			saveData.GameVersion = Application.version;
			saveData.BackupKind = backupKind;
			saveData.SaveCreatedUtcTicks = DateTime.UtcNow.Ticks;
			saveData.SaveTransactionId = saveTransactionId;
			RecoveryMeta recoveryMeta = LoadOrCreateRecoveryMeta(CurrentSlotId, RuntimeData);
			saveData.SessionBaselineUtcTicks = recoveryMeta.BaselineUtcTicks;
			saveData.SessionId = recoveryMeta.SessionId;
			if (SingletonMonoScope<PlayerManager>.HasInstance && SingletonMonoScope<LevelManager>.HasInstance && SingletonMonoScope<TalentManager>.HasInstance && SingletonMonoScope<ACTbar>.HasInstance && SingletonMonoScope<InventoryManager>.HasInstance && SingletonMonoScope<DialogManager>.HasInstance)
			{
				saveData.PlayTimeSeconds = PlayTimeManager.GetTotalSeconds();
				saveData.PlayerData = SingletonMonoScope<PlayerManager>.Instance.ExportSaveData();
				saveData.TalentData = SingletonMonoScope<TalentManager>.Instance.ExportSaveData();
				saveData.ActbarData = SingletonMonoScope<ACTbar>.Instance.ExportSaveData();
				saveData.InventoryData = SingletonMonoScope<InventoryManager>.Instance.ExportSaveData();
				saveData.DialogData = SingletonMonoScope<DialogManager>.Instance.ExportSaveData();
				saveData.LastPlayLevelId = LevelManager.GetCurLevel();
				saveData.UnlockedChapterIds = new HashSet<int>(RuntimeData.UnlockedChapterIds);
				saveData.UnlockedLevelIds = new HashSet<string>(RuntimeData.UnlockedLevelIds);
				saveData.DefeatedBossLevelIds = new HashSet<string>(RuntimeData.DefeatedBossLevelIds);
				saveData.UnlockedMijing = RuntimeData.UnlockedMijing;
				saveData.mijingFloor_easy = RuntimeData.mijingFloor_easy;
				saveData.mijingFloor_hard = RuntimeData.mijingFloor_hard;
				saveData.mijingFloor_medium = RuntimeData.mijingFloor_medium;
				saveData.mijingFloor_master = RuntimeData.mijingFloor_master;
				SaveDataEquipmentSanitizer.PrepareForWrite(saveData);
			}
			else
			{
				LogUtil.Warn("保存时部分运行时管理器缺失，已使用 RuntimeData 克隆作为 slot 快照，避免默认数据覆盖存档。");
			}
			saveData.PostLoadFix();
			GlobalSaveData globalSaveData = ((RuntimeGlobalData != null) ? DataUtil.DeepClone(RuntimeGlobalData) : GlobalSaveData.CreateNew());
			if (globalSaveData == null)
			{
				globalSaveData = GlobalSaveData.CreateNew();
			}
			if (SingletonMonoScope<WarehouseManager>.HasInstance)
			{
				globalSaveData.GlobalChestData = SingletonMonoScope<WarehouseManager>.Instance.ExportGlobalSaveData();
			}
			globalSaveData.LastWriterSlotId = CurrentSlotId;
			globalSaveData.SaveTransactionId = saveTransactionId;
			globalSaveData.SaveCreatedUtcTicks = saveData.SaveCreatedUtcTicks;
			globalSaveData.PostLoadFix();
			saveData.EmbeddedGlobalData = globalSaveData;
			string saveRoot = SaveRoot;
			string slotBackupPath = GetSlotBackupPath(CurrentSlotId, backupKind);
			string globalSavePath = Path.Combine(saveRoot, "global.sav");
			return new SaveSnapshot
			{
				SlotId = CurrentSlotId,
				BackupKind = backupKind,
				SaveRootPath = saveRoot,
				SlotPath = slotBackupPath,
				GlobalSavePath = globalSavePath,
				RecoveryMetaPath = GetRecoveryMetaPath(CurrentSlotId),
				RecoveryMetaBackupPath = GetRecoveryMetaBackupPath(CurrentSlotId),
				ManifestPath = GetManifestPath(saveRoot),
				ManifestBackupPath = GetManifestBackupPath(saveRoot),
				SlotData = saveData,
				GlobalData = globalSaveData,
				Meta = recoveryMeta
			};
		}
		catch (Exception arg)
		{
			LogUtil.Error($"构建存档快照失败: {arg}");
			return null;
		}
	}

	private static async UniTask WaitForCurrentSaveAsync()
	{
		Task task = currentSaveTask;
		if (task != null)
		{
			await task;
			return;
		}
		while (IsSaving)
		{
			await UniTask.Yield(PlayerLoopTiming.Update);
		}
	}

	private static void WaitForCurrentSaveBlocking()
	{
		SaveTaskState saveTaskState = currentSaveState;
		if (saveTaskState?.DiskTask != null)
		{
			try
			{
				saveTaskState.DiskTask.Wait();
				CompleteSnapshotSaveOnce(saveTaskState);
				return;
			}
			catch (AggregateException ex)
			{
				FailSnapshotSaveOnce(saveTaskState, $"等待当前保存完成失败：{ex.Flatten()}");
				return;
			}
			finally
			{
				ClearCurrentSaveState(saveTaskState);
			}
		}
		while (isSaving)
		{
			Task.Delay(1).Wait();
		}
	}

	private static void QueueExitSaveAfterCurrentSave()
	{
		SaveAndExitAfterCurrentSaveAsync().Forget();
	}

	private static async UniTask SaveAndExitAfterCurrentSaveAsync()
	{
		await WaitForCurrentSaveAsync();
		if (!SaveCurrentGameAsync(SaveBackupKind.ExitBackup))
		{
			LogUtil.Warn("当前保存完成后执行保存并退出备份失败。");
		}
	}

	private static bool SaveCurrentGameBlocking(SaveBackupKind backupKind)
	{
		if (!IsRuntimeSaveBackupKind(backupKind))
		{
			LogUtil.Error($"非法运行时保存类型：{backupKind}。运行时只允许写入自动备份或保存并退出备份，禁止覆盖进入基线。");
			return false;
		}
		if (CurrentSlotId < 0)
		{
			return false;
		}
		if (SceneManager.GetActiveScene().name == "StartScene")
		{
			return false;
		}
		if (!HasRuntime)
		{
			LogUtil.Error("SaveRuntime 未初始化，无法保存");
			return false;
		}
		if (isSaving)
		{
			LogUtil.Warn("当前已有存档任务进行中，本次同步保存跳过。");
			return false;
		}
		SaveSnapshot saveSnapshot = BuildSaveSnapshot(backupKind);
		if (saveSnapshot == null)
		{
			LogUtil.Error("构建存档快照失败，无法执行同步保存。");
			return false;
		}
		isSaving = true;
		lastSaveSucceeded = false;
		try
		{
			WriteSnapshotToDisk(saveSnapshot);
			CompleteSnapshotSave(saveSnapshot);
			return true;
		}
		catch (Exception arg)
		{
			lastSaveSucceeded = false;
			LogUtil.Error($"同步保存失败 slot_{saveSnapshot.SlotId}: {arg}");
			return false;
		}
		finally
		{
			isSaving = false;
			currentSaveTask = null;
			currentSaveState = null;
		}
	}

	private static async Task CompleteSaveAfterDiskWriteAsync(SaveTaskState saveState)
	{
		try
		{
			await saveState.DiskTask;
			CompleteSnapshotSaveOnce(saveState);
		}
		catch (Exception arg)
		{
			FailSnapshotSaveOnce(saveState, $"异步保存失败 slot_{saveState?.Snapshot?.SlotId}: {arg}");
		}
		finally
		{
			ClearCurrentSaveState(saveState);
		}
	}

	private static void WriteSnapshotToDisk(SaveSnapshot snapshot)
	{
		if (!Directory.Exists(snapshot.SaveRootPath))
		{
			Directory.CreateDirectory(snapshot.SaveRootPath);
		}
		AtomicSave(snapshot.GlobalSavePath, snapshot.GlobalData);
		AtomicSave(snapshot.SlotPath, snapshot.SlotData);
		if (snapshot.BackupKind == SaveBackupKind.AutoBackup)
		{
			snapshot.Meta.LastAutoBackupUtcTicks = snapshot.SlotData.SaveCreatedUtcTicks;
		}
		else if (snapshot.BackupKind == SaveBackupKind.ExitBackup)
		{
			snapshot.Meta.LastExitBackupUtcTicks = snapshot.SlotData.SaveCreatedUtcTicks;
		}
		SaveRecoveryMeta(snapshot.RecoveryMetaPath, snapshot.RecoveryMetaBackupPath, snapshot.Meta);
		UpdateManifestAfterSnapshot(snapshot);
	}

	private static void CompleteSnapshotSave(SaveSnapshot snapshot)
	{
		RuntimeData = snapshot.SlotData;
		RuntimeGlobalData = snapshot.GlobalData;
		lastSaveSucceeded = true;
		LogUtil.Info($"保存完成 slot_{snapshot.SlotId} ({snapshot.BackupKind})");
	}

	private static void CompleteSnapshotSaveOnce(SaveTaskState saveState)
	{
		if (saveState?.Snapshot == null)
		{
			return;
		}
		lock (saveStateLock)
		{
			if (!saveState.CompletionHandled)
			{
				CompleteSnapshotSave(saveState.Snapshot);
				saveState.CompletionHandled = true;
			}
		}
	}

	private static void FailSnapshotSaveOnce(SaveTaskState saveState, string message)
	{
		bool flag = false;
		lock (saveStateLock)
		{
			if (saveState == null || !saveState.CompletionHandled)
			{
				if (saveState != null)
				{
					saveState.CompletionHandled = true;
				}
				lastSaveSucceeded = false;
				flag = true;
			}
		}
		if (flag)
		{
			LogUtil.Error(message);
		}
	}

	private static void ClearCurrentSaveState(SaveTaskState saveState)
	{
		lock (saveStateLock)
		{
			if (saveState == null || currentSaveState == saveState)
			{
				isSaving = false;
				currentSaveTask = null;
				currentSaveState = null;
			}
		}
	}

	private static void ResetCurrentSlotIndex()
	{
		CurrentSlotId = -1;
	}

	private static void SetCurrentSlot(int slotId)
	{
		CurrentSlotId = -1;
		if (!HasSaveSlot(slotId))
		{
			LogUtil.Error($"槽位Id：{slotId} 无数据。");
		}
		else
		{
			CurrentSlotId = slotId;
		}
	}

	private static bool TryResolveSlotForEntry(int slotId, out SaveData data, out string loadedPath)
	{
		data = null;
		loadedPath = null;
		SaveData data2;
		string loadedPath2;
		bool flag = TryLoadValidSlotData(GetSlotCandidatePaths(slotId, SaveBackupKind.EntryBaseline), SaveBackupKind.EntryBaseline, 0L, null, out data2, out loadedPath2);
		RecoveryMeta recoveryMeta = null;
		if (TryLoadRecoveryMeta(GetRecoveryMetaPath(slotId), slotId, data2, out var meta) || TryLoadRecoveryMeta(GetRecoveryMetaBackupPath(slotId), slotId, data2, out meta))
		{
			recoveryMeta = meta;
		}
		if (flag && TryBuildRecoveryMetaFromBaseline(slotId, data2, out var meta2) && (recoveryMeta == null || recoveryMeta.BaselineUtcTicks != meta2.BaselineUtcTicks || recoveryMeta.SessionId != meta2.SessionId))
		{
			recoveryMeta = meta2;
		}
		else if (recoveryMeta == null && flag)
		{
			recoveryMeta = LoadOrCreateRecoveryMeta(slotId, data2);
		}
		if (recoveryMeta != null && TryLoadValidSlotData(GetSlotCandidatePaths(slotId, SaveBackupKind.ExitBackup), SaveBackupKind.ExitBackup, recoveryMeta.BaselineUtcTicks, recoveryMeta.SessionId, out var data3, out var loadedPath3))
		{
			data = data3;
			loadedPath = loadedPath3;
			return true;
		}
		if (recoveryMeta != null && TryLoadValidSlotData(GetSlotCandidatePaths(slotId, SaveBackupKind.AutoBackup), SaveBackupKind.AutoBackup, recoveryMeta.BaselineUtcTicks, recoveryMeta.SessionId, out var data4, out var loadedPath4))
		{
			data = data4;
			loadedPath = loadedPath4;
			return true;
		}
		if (!flag && recoveryMeta == null && TryLoadBestOrphanBackup(slotId, out var data5, out var loadedPath5))
		{
			data = data5;
			loadedPath = loadedPath5;
			LogUtil.Warn($"slot_{slotId} 缺少基线与恢复元数据，已从孤立备份恢复：{Path.GetFileName(loadedPath5)}");
			return true;
		}
		if (flag)
		{
			data = data2;
			loadedPath = loadedPath2;
			return true;
		}
		LogUtil.Error($"slot_{slotId} 基线、自动备份、退出备份均损坏或不存在，无法恢复。");
		return false;
	}

	private static bool TryPeekSlotForEntry(int slotId, out SaveData data, out string loadedPath)
	{
		data = null;
		loadedPath = null;
		SaveData data2;
		string loadedPath2;
		bool flag = TryLoadValidSlotData(GetSlotCandidatePaths(slotId, SaveBackupKind.EntryBaseline), SaveBackupKind.EntryBaseline, 0L, null, out data2, out loadedPath2);
		RecoveryMeta recoveryMeta = null;
		if (TryLoadRecoveryMeta(GetRecoveryMetaPath(slotId), slotId, data2, out var meta) || TryLoadRecoveryMeta(GetRecoveryMetaBackupPath(slotId), slotId, data2, out meta))
		{
			recoveryMeta = meta;
		}
		if (flag && TryBuildRecoveryMetaFromBaseline(slotId, data2, out var meta2) && (recoveryMeta == null || recoveryMeta.BaselineUtcTicks != meta2.BaselineUtcTicks || recoveryMeta.SessionId != meta2.SessionId))
		{
			recoveryMeta = meta2;
		}
		if (recoveryMeta != null && TryLoadValidSlotData(GetSlotCandidatePaths(slotId, SaveBackupKind.ExitBackup), SaveBackupKind.ExitBackup, recoveryMeta.BaselineUtcTicks, recoveryMeta.SessionId, out var data3, out var loadedPath3))
		{
			data = data3;
			loadedPath = loadedPath3;
			return true;
		}
		if (recoveryMeta != null && TryLoadValidSlotData(GetSlotCandidatePaths(slotId, SaveBackupKind.AutoBackup), SaveBackupKind.AutoBackup, recoveryMeta.BaselineUtcTicks, recoveryMeta.SessionId, out var data4, out var loadedPath4))
		{
			data = data4;
			loadedPath = loadedPath4;
			return true;
		}
		if (!flag && recoveryMeta == null && TryLoadBestOrphanBackup(slotId, out var data5, out var loadedPath5))
		{
			data = data5;
			loadedPath = loadedPath5;
			return true;
		}
		if (flag)
		{
			data = data2;
			loadedPath = loadedPath2;
			return true;
		}
		return false;
	}

	private static bool TryLoadValidSlotData(string path, SaveBackupKind expectedKind, long expectedBaselineUtcTicks, string expectedSessionId, out SaveData data)
	{
		string loadedPath;
		return TryLoadValidSlotData(new string[1] { path }, expectedKind, expectedBaselineUtcTicks, expectedSessionId, out data, out loadedPath);
	}

	private static bool TryLoadValidSlotData(IEnumerable<string> paths, SaveBackupKind expectedKind, long expectedBaselineUtcTicks, string expectedSessionId, out SaveData data, out string loadedPath)
	{
		data = null;
		loadedPath = null;
		SaveData saveData = null;
		string text = null;
		long num = long.MinValue;
		DateTime dateTime = DateTime.MinValue;
		foreach (string item in paths.SelectMany(GetRecoverablePaths))
		{
			try
			{
				if (!TryLoadSaveFile<SaveData>(item, out data))
				{
					data = null;
				}
			}
			catch (Exception arg)
			{
				LogUtil.Warn($"读取存档失败，path = {item}\n{arg}");
				continue;
			}
			if (data == null)
			{
				LogUtil.Warn("存档为空或反序列化失败，path = " + item);
				continue;
			}
			bool flag = data.SaveCreatedUtcTicks > 0;
			try
			{
				data.PostLoadFix();
				if (!flag)
				{
					data.SaveCreatedUtcTicks = SafeGetLastWriteTimeUtc(item).Ticks;
				}
				if (IsValidSessionId(data.SessionId))
				{
					data.SessionId = NormalizeSessionId(data.SessionId);
				}
			}
			catch (Exception arg2)
			{
				LogUtil.Warn($"存档 PostLoadFix 失败，path = {item}\n{arg2}");
				continue;
			}
			if (!IsDefinedBackupKind(data.BackupKind))
			{
				LogUtil.Warn($"存档备份类型非法，path = {item}, actual = {data.BackupKind}");
				continue;
			}
			if (!IsSaveDataStructurallyValid(data))
			{
				LogUtil.Warn("存档结构校验失败，path = " + item);
				continue;
			}
			if (data.BackupKind != expectedKind)
			{
				LogUtil.Warn($"存档备份类型不匹配，path = {item}, expected = {expectedKind}, actual = {data.BackupKind}");
				continue;
			}
			if (expectedKind != 0)
			{
				if (expectedBaselineUtcTicks <= 0 || data.SessionBaselineUtcTicks != expectedBaselineUtcTicks)
				{
					LogUtil.Warn("备份存档基线时间戳不匹配，path = " + item);
					continue;
				}
				if (string.IsNullOrEmpty(expectedSessionId) || data.SessionId != expectedSessionId)
				{
					LogUtil.Warn("备份存档会话 Id 不匹配，path = " + item);
					continue;
				}
				if (data.SaveCreatedUtcTicks <= data.SessionBaselineUtcTicks)
				{
					LogUtil.Warn("备份存档时间戳早于或等于基线，path = " + item);
					continue;
				}
			}
			long saveCreatedUtcTicks = data.SaveCreatedUtcTicks;
			DateTime dateTime2 = SafeGetLastWriteTimeUtc(item);
			if (saveData == null || saveCreatedUtcTicks > num || (saveCreatedUtcTicks == num && dateTime2 > dateTime))
			{
				saveData = data;
				text = item;
				num = saveCreatedUtcTicks;
				dateTime = dateTime2;
			}
		}
		data = saveData;
		loadedPath = text;
		return data != null;
	}

	private static bool TryLoadBestOrphanBackup(int slotId, out SaveData data, out string loadedPath)
	{
		data = null;
		loadedPath = null;
		SaveData bestData = null;
		string bestPath = null;
		long bestTicks = long.MinValue;
		DateTime bestFileTime = DateTime.MinValue;
		TryConsiderOrphanBackup(GetSlotCandidatePaths(slotId, SaveBackupKind.ExitBackup), SaveBackupKind.ExitBackup, ref bestData, ref bestPath, ref bestTicks, ref bestFileTime);
		TryConsiderOrphanBackup(GetSlotCandidatePaths(slotId, SaveBackupKind.AutoBackup), SaveBackupKind.AutoBackup, ref bestData, ref bestPath, ref bestTicks, ref bestFileTime);
		data = bestData;
		loadedPath = bestPath;
		return data != null;
	}

	private static void TryConsiderOrphanBackup(IEnumerable<string> paths, SaveBackupKind expectedKind, ref SaveData bestData, ref string bestPath, ref long bestTicks, ref DateTime bestFileTime)
	{
		foreach (string item in paths.SelectMany(GetRecoverablePaths))
		{
			SaveData data;
			try
			{
				if (!TryLoadSaveFile<SaveData>(item, out data))
				{
					data = null;
				}
			}
			catch (Exception arg)
			{
				LogUtil.Warn($"读取孤立备份失败，path = {item}\n{arg}");
				continue;
			}
			if (data == null)
			{
				continue;
			}
			bool flag = data.SaveCreatedUtcTicks > 0;
			try
			{
				data.PostLoadFix();
				if (!flag)
				{
					data.SaveCreatedUtcTicks = SafeGetLastWriteTimeUtc(item).Ticks;
				}
				if (IsValidSessionId(data.SessionId))
				{
					data.SessionId = NormalizeSessionId(data.SessionId);
				}
			}
			catch (Exception arg2)
			{
				LogUtil.Warn($"孤立备份 PostLoadFix 失败，path = {item}\n{arg2}");
				continue;
			}
			if (IsSaveDataStructurallyValid(data) && IsDefinedBackupKind(data.BackupKind) && data.BackupKind == expectedKind && data.SessionBaselineUtcTicks > 0 && !string.IsNullOrEmpty(data.SessionId) && data.SaveCreatedUtcTicks > data.SessionBaselineUtcTicks)
			{
				DateTime dateTime = SafeGetLastWriteTimeUtc(item);
				if (bestData == null || data.SaveCreatedUtcTicks > bestTicks || (data.SaveCreatedUtcTicks == bestTicks && dateTime > bestFileTime))
				{
					bestData = data;
					bestPath = item;
					bestTicks = data.SaveCreatedUtcTicks;
					bestFileTime = dateTime;
				}
			}
		}
	}

	private static bool IsSaveDataStructurallyValid(SaveData data)
	{
		if (data == null)
		{
			return false;
		}
		if (data.SaveCreatedUtcTicks <= 0)
		{
			return false;
		}
		if (data.PlayerData == null)
		{
			return false;
		}
		if (data.TalentData == null || data.ActbarData == null || data.InventoryData == null || data.DialogData == null)
		{
			return false;
		}
		if (data.UnlockedChapterIds == null || data.UnlockedLevelIds == null || data.DefeatedBossLevelIds == null)
		{
			return false;
		}
		return true;
	}

	private static bool IsDefinedBackupKind(SaveBackupKind backupKind)
	{
		if (backupKind != 0 && backupKind != SaveBackupKind.AutoBackup)
		{
			return backupKind == SaveBackupKind.ExitBackup;
		}
		return true;
	}

	private static bool IsRuntimeSaveBackupKind(SaveBackupKind backupKind)
	{
		if (backupKind != SaveBackupKind.AutoBackup)
		{
			return backupKind == SaveBackupKind.ExitBackup;
		}
		return true;
	}

	private static long GetBaselineTicksFromData(SaveData data)
	{
		if (data == null)
		{
			return 0L;
		}
		if (data.SessionBaselineUtcTicks > 0)
		{
			return data.SessionBaselineUtcTicks;
		}
		if (data.SaveCreatedUtcTicks <= 0)
		{
			return 0L;
		}
		return data.SaveCreatedUtcTicks;
	}

	private static bool TryBuildRecoveryMetaFromBaseline(int slotId, SaveData baselineData, out RecoveryMeta meta)
	{
		meta = null;
		if (baselineData == null || string.IsNullOrEmpty(baselineData.SessionId) || !IsValidSessionId(baselineData.SessionId))
		{
			return false;
		}
		long baselineTicksFromData = GetBaselineTicksFromData(baselineData);
		if (baselineTicksFromData <= 0)
		{
			return false;
		}
		meta = new RecoveryMeta
		{
			Version = 1,
			SlotId = slotId,
			BaselineUtcTicks = baselineTicksFromData,
			SessionId = NormalizeSessionId(baselineData.SessionId),
			LastAutoBackupUtcTicks = 0L,
			LastExitBackupUtcTicks = 0L
		};
		return true;
	}

	private static bool TryLoadValidGlobalData(string path, out GlobalSaveData data, out string loadedPath)
	{
		return TryLoadValidGlobalData(new string[1] { path }, out data, out loadedPath);
	}

	private static bool TryLoadValidGlobalData(IEnumerable<string> paths, out GlobalSaveData data, out string loadedPath)
	{
		data = null;
		loadedPath = null;
		GlobalSaveData globalSaveData = null;
		string text = null;
		long num = long.MinValue;
		DateTime dateTime = DateTime.MinValue;
		foreach (string item in paths.SelectMany(GetRecoverablePaths))
		{
			try
			{
				if (!TryLoadSaveFile<GlobalSaveData>(item, out data))
				{
					data = null;
				}
			}
			catch (Exception arg)
			{
				LogUtil.Warn($"读取全局存档失败，path = {item}\n{arg}");
				continue;
			}
			if (data == null)
			{
				continue;
			}
			bool flag = data.SaveCreatedUtcTicks > 0;
			try
			{
				data.PostLoadFix();
				if (!flag)
				{
					data.SaveCreatedUtcTicks = SafeGetLastWriteTimeUtc(item).Ticks;
				}
			}
			catch (Exception arg2)
			{
				LogUtil.Warn($"全局存档 PostLoadFix 失败，path = {item}\n{arg2}");
				continue;
			}
			if (data.GlobalChestData != null && data.GlobalChestData.ChestItems != null)
			{
				long num2 = ((data.SaveCreatedUtcTicks > 0) ? data.SaveCreatedUtcTicks : SafeGetLastWriteTimeUtc(item).Ticks);
				DateTime dateTime2 = SafeGetLastWriteTimeUtc(item);
				if (globalSaveData == null || num2 > num || (num2 == num && dateTime2 > dateTime))
				{
					globalSaveData = data;
					text = item;
					num = num2;
					dateTime = dateTime2;
				}
			}
		}
		data = globalSaveData;
		loadedPath = text;
		return data != null;
	}

	private static bool BeginNewSession(int slotId, SaveData selectedData)
	{
		selectedData.PostLoadFix();
		long ticks = DateTime.UtcNow.Ticks;
		string sessionId = Guid.NewGuid().ToString("N");
		string gameVersion = selectedData.GameVersion;
		SaveBackupKind backupKind = selectedData.BackupKind;
		long saveCreatedUtcTicks = selectedData.SaveCreatedUtcTicks;
		long sessionBaselineUtcTicks = selectedData.SessionBaselineUtcTicks;
		string sessionId2 = selectedData.SessionId;
		string saveTransactionId = selectedData.SaveTransactionId;
		GlobalSaveData embeddedGlobalData = selectedData.EmbeddedGlobalData;
		selectedData.BackupKind = SaveBackupKind.EntryBaseline;
		selectedData.SaveCreatedUtcTicks = ticks;
		selectedData.SessionBaselineUtcTicks = ticks;
		selectedData.SessionId = sessionId;
		selectedData.SaveTransactionId = "";
		if (RuntimeGlobalData != null)
		{
			selectedData.EmbeddedGlobalData = RuntimeGlobalData;
		}
		RecoveryMeta meta = new RecoveryMeta
		{
			Version = 1,
			SlotId = slotId,
			BaselineUtcTicks = ticks,
			SessionId = sessionId,
			LastAutoBackupUtcTicks = 0L,
			LastExitBackupUtcTicks = 0L
		};
		if (!SaveToSlot(slotId, selectedData))
		{
			selectedData.GameVersion = gameVersion;
			selectedData.BackupKind = backupKind;
			selectedData.SaveCreatedUtcTicks = saveCreatedUtcTicks;
			selectedData.SessionBaselineUtcTicks = sessionBaselineUtcTicks;
			selectedData.SessionId = sessionId2;
			selectedData.SaveTransactionId = saveTransactionId;
			selectedData.EmbeddedGlobalData = embeddedGlobalData;
			LogUtil.Error($"slot_{slotId} 进入游戏基线写入失败，将继续使用内存中的恢复数据。");
			return false;
		}
		SaveRecoveryMeta(GetRecoveryMetaPath(slotId), GetRecoveryMetaBackupPath(slotId), meta);
		UpdateManifestForBaseline(slotId, selectedData, GetSlotPath(slotId));
		return true;
	}

	private static RecoveryMeta LoadOrCreateRecoveryMeta(int slotId, SaveData baselineData)
	{
		RecoveryMeta meta;
		bool flag = TryBuildRecoveryMetaFromBaseline(slotId, baselineData, out meta);
		if ((TryLoadRecoveryMeta(GetRecoveryMetaPath(slotId), slotId, baselineData, out var meta2) || TryLoadRecoveryMeta(GetRecoveryMetaBackupPath(slotId), slotId, baselineData, out meta2)) && (!flag || (meta2.BaselineUtcTicks == meta.BaselineUtcTicks && meta2.SessionId == meta.SessionId)))
		{
			return meta2;
		}
		if (flag)
		{
			SaveRecoveryMeta(GetRecoveryMetaPath(slotId), GetRecoveryMetaBackupPath(slotId), meta);
			return meta;
		}
		meta2 = new RecoveryMeta();
		meta2.Normalize(slotId, baselineData);
		SaveRecoveryMeta(GetRecoveryMetaPath(slotId), GetRecoveryMetaBackupPath(slotId), meta2);
		return meta2;
	}

	private static bool TryLoadRecoveryMeta(string path, int slotId, SaveData baselineData, out RecoveryMeta meta)
	{
		meta = null;
		RecoveryMeta recoveryMeta = null;
		long num = long.MinValue;
		long num2 = long.MinValue;
		DateTime dateTime = DateTime.MinValue;
		foreach (string recoverablePath in GetRecoverablePaths(path))
		{
			try
			{
				using FileStream fileStream = new FileStream(recoverablePath, FileMode.Open, FileAccess.Read, FileShare.Read);
				using BinaryReader binaryReader = new BinaryReader(fileStream);
				if (fileStream.Length != 56)
				{
					continue;
				}
				int num3 = binaryReader.ReadInt32();
				int num4 = binaryReader.ReadInt32();
				long num5 = binaryReader.ReadInt64();
				long num6 = binaryReader.ReadInt64();
				long num7 = binaryReader.ReadInt64();
				int num8 = binaryReader.ReadInt32();
				int num9 = binaryReader.ReadInt32();
				string text = new Guid(binaryReader.ReadBytes(16)).ToString("N");
				int num10 = BuildRecoveryMetaCheck(num3, num4, num5, num6, num7, num8);
				if (num9 == num10 && num8 == GetStableStringHash(text) && num3 == 1 && num4 == slotId)
				{
					RecoveryMeta recoveryMeta2 = new RecoveryMeta
					{
						Version = num3,
						SlotId = num4,
						BaselineUtcTicks = num5,
						SessionId = text,
						LastAutoBackupUtcTicks = num6,
						LastExitBackupUtcTicks = num7
					};
					recoveryMeta2.Normalize(slotId, baselineData);
					long num11 = Math.Max(recoveryMeta2.LastAutoBackupUtcTicks, recoveryMeta2.LastExitBackupUtcTicks);
					DateTime dateTime2 = SafeGetLastWriteTimeUtc(recoverablePath);
					if (recoveryMeta == null || recoveryMeta2.BaselineUtcTicks > num || (recoveryMeta2.BaselineUtcTicks == num && num11 > num2) || (recoveryMeta2.BaselineUtcTicks == num && num11 == num2 && dateTime2 > dateTime))
					{
						recoveryMeta = recoveryMeta2;
						num = recoveryMeta2.BaselineUtcTicks;
						num2 = num11;
						dateTime = dateTime2;
					}
				}
			}
			catch (Exception ex)
			{
				LogUtil.Warn("读取恢复元数据失败，path = " + recoverablePath + ", reason = " + ex.Message);
			}
		}
		meta = recoveryMeta;
		return meta != null;
	}

	private static SaveManifest LoadSaveManifest()
	{
		return LoadSaveManifest(ManifestPath);
	}

	private static SaveManifest LoadSaveManifest(string manifestPath)
	{
		if (TryLoadSaveManifest(manifestPath, out var manifest))
		{
			return manifest;
		}
		SaveManifest saveManifest = new SaveManifest();
		saveManifest.Normalize();
		return saveManifest;
	}

	private static bool TryLoadSaveManifest(string manifestPath, out SaveManifest manifest)
	{
		manifest = null;
		SaveManifest saveManifest = null;
		long num = long.MinValue;
		DateTime dateTime = DateTime.MinValue;
		foreach (string recoverablePath in GetRecoverablePaths(manifestPath))
		{
			try
			{
				if (TryReadSaveManifest(recoverablePath, out var manifest2))
				{
					DateTime dateTime2 = SafeGetLastWriteTimeUtc(recoverablePath);
					long num2 = ((manifest2.UpdatedUtcTicks > 0) ? manifest2.UpdatedUtcTicks : dateTime2.Ticks);
					if (saveManifest == null || num2 > num || (num2 == num && dateTime2 > dateTime))
					{
						saveManifest = manifest2;
						num = num2;
						dateTime = dateTime2;
					}
				}
			}
			catch (Exception ex)
			{
				LogUtil.Warn("读取存档 manifest 失败：" + recoverablePath + ", reason = " + ex.Message);
			}
		}
		manifest = saveManifest;
		return manifest != null;
	}

	private static bool TryReadSaveManifest(string path, out SaveManifest manifest)
	{
		manifest = null;
		if (string.IsNullOrEmpty(path) || !File.Exists(path))
		{
			return false;
		}
		using FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
		using BinaryReader binaryReader = new BinaryReader(fileStream);
		if (fileStream.Length < 16)
		{
			return false;
		}
		int num = binaryReader.ReadInt32();
		int num2 = binaryReader.ReadInt32();
		int num3 = binaryReader.ReadInt32();
		int num4 = binaryReader.ReadInt32();
		if (num != 1296454477 || num2 < 1 || num2 > 3 || num4 <= 0 || num4 > 1048576)
		{
			return false;
		}
		byte[] array = binaryReader.ReadBytes(num4);
		if (array.Length != num4)
		{
			return false;
		}
		if (BuildManifestPayloadCheck(array, num2) != num3)
		{
			return false;
		}
		manifest = ReadManifestPayload(array, num2);
		manifest.Normalize();
		return true;
	}

	private static SaveManifest ReadManifestPayload(byte[] payload, int version)
	{
		SaveManifest saveManifest = new SaveManifest
		{
			Version = version
		};
		using MemoryStream input = new MemoryStream(payload);
		using BinaryReader binaryReader = new BinaryReader(input);
		saveManifest.UpdatedUtcTicks = binaryReader.ReadInt64();
		saveManifest.GlobalPath = binaryReader.ReadString();
		saveManifest.GlobalTicks = binaryReader.ReadInt64();
		saveManifest.GlobalTransactionId = binaryReader.ReadString();
		int val = binaryReader.ReadInt32();
		val = Math.Max(0, Math.Min(val, 256));
		saveManifest.Slots = new List<SaveManifestSlotEntry>(val);
		for (int i = 0; i < val; i++)
		{
			SaveManifestSlotEntry saveManifestSlotEntry = new SaveManifestSlotEntry
			{
				SlotId = binaryReader.ReadInt32(),
				BaselinePath = binaryReader.ReadString(),
				AutoPath = binaryReader.ReadString(),
				ExitPath = binaryReader.ReadString(),
				BaselineTicks = binaryReader.ReadInt64(),
				AutoTicks = binaryReader.ReadInt64(),
				ExitTicks = binaryReader.ReadInt64(),
				SessionId = binaryReader.ReadString(),
				UpdatedUtcTicks = binaryReader.ReadInt64()
			};
			if (version >= 2)
			{
				saveManifestSlotEntry.UiGameVersion = binaryReader.ReadString();
				saveManifestSlotEntry.UiPlayTimeSeconds = binaryReader.ReadInt64();
				saveManifestSlotEntry.UiPlayerType = binaryReader.ReadInt32();
				saveManifestSlotEntry.UiPlayerName = binaryReader.ReadString();
				saveManifestSlotEntry.UiLevel = binaryReader.ReadInt32();
				if (version >= 3)
				{
					saveManifestSlotEntry.UiDFLevel = binaryReader.ReadInt32();
				}
			}
			saveManifest.Slots.Add(saveManifestSlotEntry);
		}
		return saveManifest;
	}

	private static byte[] BuildManifestPayload(SaveManifest manifest)
	{
		manifest.Normalize();
		using MemoryStream memoryStream = new MemoryStream();
		using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(manifest.UpdatedUtcTicks);
		binaryWriter.Write(manifest.GlobalPath ?? "");
		binaryWriter.Write(manifest.GlobalTicks);
		binaryWriter.Write(manifest.GlobalTransactionId ?? "");
		List<SaveManifestSlotEntry> list = (from s in manifest.Slots
			where s != null && s.SlotId >= 0
			orderby s.SlotId
			select s).Take(256).ToList();
		binaryWriter.Write(list.Count);
		foreach (SaveManifestSlotEntry item in list)
		{
			item.Normalize();
			binaryWriter.Write(item.SlotId);
			binaryWriter.Write(item.BaselinePath ?? "");
			binaryWriter.Write(item.AutoPath ?? "");
			binaryWriter.Write(item.ExitPath ?? "");
			binaryWriter.Write(item.BaselineTicks);
			binaryWriter.Write(item.AutoTicks);
			binaryWriter.Write(item.ExitTicks);
			binaryWriter.Write(item.SessionId ?? "");
			binaryWriter.Write(item.UpdatedUtcTicks);
			binaryWriter.Write(item.UiGameVersion ?? "");
			binaryWriter.Write(item.UiPlayTimeSeconds);
			binaryWriter.Write(item.UiPlayerType);
			binaryWriter.Write(item.UiPlayerName ?? "");
			binaryWriter.Write(item.UiLevel);
			binaryWriter.Write(item.UiDFLevel);
		}
		binaryWriter.Flush();
		return memoryStream.ToArray();
	}

	private static int BuildManifestPayloadCheck(byte[] payload, int version)
	{
		int num = 1511506142;
		num = (num * 397) ^ version;
		if (payload != null)
		{
			for (int i = 0; i < payload.Length; i++)
			{
				num = (num * 31) ^ payload[i];
			}
		}
		return num;
	}

	private static bool SaveManifestBestEffort(SaveManifest manifest, string manifestPath, string backupPath)
	{
		if (manifest == null)
		{
			return false;
		}
		try
		{
			Directory.CreateDirectory(Path.GetDirectoryName(manifestPath) ?? string.Empty);
			manifest.UpdatedUtcTicks = DateTime.UtcNow.Ticks;
			manifest.Normalize();
			byte[] array = BuildManifestPayload(manifest);
			int value = BuildManifestPayloadCheck(array, 3);
			string text = manifestPath + ".tmp";
			if (File.Exists(text))
			{
				File.Delete(text);
			}
			using (FileStream fileStream = new FileStream(text, FileMode.CreateNew, FileAccess.Write, FileShare.None))
			{
				using BinaryWriter binaryWriter = new BinaryWriter(fileStream);
				binaryWriter.Write(1296454477);
				binaryWriter.Write(3);
				binaryWriter.Write(value);
				binaryWriter.Write(array.Length);
				binaryWriter.Write(array);
				binaryWriter.Flush();
				fileStream.Flush();
			}
			if (!TryReadSaveManifest(text, out var _))
			{
				throw new IOException("manifest 临时文件校验失败：" + text);
			}
			ReplaceFileKeepingBackup(text, manifestPath, backupPath);
			return true;
		}
		catch (Exception ex)
		{
			LogUtil.Warn("保存存档 manifest 失败：" + ex.Message);
			return false;
		}
	}

	private static void UpdateManifestAfterSnapshot(SaveSnapshot snapshot)
	{
		SaveManifest saveManifest = LoadSaveManifest(snapshot.ManifestPath);
		SaveManifestSlotEntry orCreateSlot = saveManifest.GetOrCreateSlot(snapshot.SlotId);
		long ticks = DateTime.UtcNow.Ticks;
		orCreateSlot.SessionId = snapshot.SlotData.SessionId;
		orCreateSlot.BaselineTicks = snapshot.Meta?.BaselineUtcTicks ?? orCreateSlot.BaselineTicks;
		orCreateSlot.UpdatedUtcTicks = ticks;
		UpdateEntryUiSummary(orCreateSlot, snapshot.SlotData);
		if (snapshot.BackupKind == SaveBackupKind.AutoBackup)
		{
			orCreateSlot.AutoPath = ToManifestRelativePath(snapshot.SlotPath, snapshot.SaveRootPath);
			orCreateSlot.AutoTicks = snapshot.SlotData.SaveCreatedUtcTicks;
		}
		else if (snapshot.BackupKind == SaveBackupKind.ExitBackup)
		{
			orCreateSlot.ExitPath = ToManifestRelativePath(snapshot.SlotPath, snapshot.SaveRootPath);
			orCreateSlot.ExitTicks = snapshot.SlotData.SaveCreatedUtcTicks;
		}
		else
		{
			orCreateSlot.BaselinePath = ToManifestRelativePath(snapshot.SlotPath, snapshot.SaveRootPath);
			orCreateSlot.BaselineTicks = snapshot.SlotData.SaveCreatedUtcTicks;
		}
		saveManifest.GlobalPath = ToManifestRelativePath(snapshot.GlobalSavePath, snapshot.SaveRootPath);
		saveManifest.GlobalTicks = snapshot.GlobalData.SaveCreatedUtcTicks;
		saveManifest.GlobalTransactionId = snapshot.GlobalData.SaveTransactionId;
		SaveManifestBestEffort(saveManifest, snapshot.ManifestPath, snapshot.ManifestBackupPath);
	}

	private static void UpdateManifestForGlobal(GlobalSaveData data, string globalPath)
	{
		if (data != null)
		{
			SaveManifest saveManifest = LoadSaveManifest();
			saveManifest.GlobalPath = ToManifestRelativePath(globalPath, SaveRoot);
			saveManifest.GlobalTicks = data.SaveCreatedUtcTicks;
			saveManifest.GlobalTransactionId = data.SaveTransactionId;
			SaveManifestBestEffort(saveManifest, ManifestPath, ManifestBackupPath);
		}
	}

	private static void UpdateManifestForBaseline(int slotId, SaveData data, string slotPath)
	{
		if (data != null)
		{
			SaveManifest saveManifest = LoadSaveManifest();
			SaveManifestSlotEntry orCreateSlot = saveManifest.GetOrCreateSlot(slotId);
			orCreateSlot.BaselinePath = ToManifestRelativePath(slotPath, SaveRoot);
			orCreateSlot.BaselineTicks = data.SaveCreatedUtcTicks;
			orCreateSlot.SessionId = data.SessionId;
			orCreateSlot.UpdatedUtcTicks = DateTime.UtcNow.Ticks;
			UpdateEntryUiSummary(orCreateSlot, data);
			SaveManifestBestEffort(saveManifest, ManifestPath, ManifestBackupPath);
		}
	}

	private static void RemoveSlotFromManifest(int slotId)
	{
		SaveManifest saveManifest = LoadSaveManifest();
		if (saveManifest.Slots.RemoveAll((SaveManifestSlotEntry s) => s != null && s.SlotId == slotId) > 0)
		{
			SaveManifestBestEffort(saveManifest, ManifestPath, ManifestBackupPath);
		}
	}

	private static bool MarkGlobalDeletedInManifest()
	{
		SaveManifest saveManifest = LoadSaveManifest();
		saveManifest.GlobalPath = "";
		saveManifest.GlobalTicks = DateTime.UtcNow.Ticks;
		saveManifest.GlobalTransactionId = "__global_deleted__";
		return SaveManifestBestEffort(saveManifest, ManifestPath, ManifestBackupPath);
	}

	private static bool IsGlobalDeleteMarkerNewerThan(long ticks)
	{
		SaveManifest saveManifest = LoadSaveManifest();
		if (saveManifest.GlobalTransactionId == "__global_deleted__" && saveManifest.GlobalTicks > 0)
		{
			return saveManifest.GlobalTicks >= ticks;
		}
		return false;
	}

	private static bool IsGlobalSuppressedByDeleteMarker(GlobalSaveData data, long ticks)
	{
		SaveManifest saveManifest = LoadSaveManifest();
		if (saveManifest.GlobalTransactionId != "__global_deleted__" || saveManifest.GlobalTicks <= 0)
		{
			return false;
		}
		if (data == null || string.IsNullOrEmpty(data.SaveTransactionId))
		{
			return true;
		}
		return saveManifest.GlobalTicks >= ticks;
	}

	private static void SaveRecoveryMeta(string path, string backupPath, RecoveryMeta meta)
	{
		if (meta == null)
		{
			return;
		}
		try
		{
			Directory.CreateDirectory(Path.GetDirectoryName(path) ?? string.Empty);
			string text = path + ".tmp";
			if (File.Exists(text))
			{
				File.Delete(text);
			}
			using (FileStream fileStream = new FileStream(text, FileMode.CreateNew, FileAccess.Write, FileShare.None))
			{
				using BinaryWriter binaryWriter = new BinaryWriter(fileStream);
				if (!IsValidSessionId(meta.SessionId))
				{
					meta.SessionId = Guid.NewGuid().ToString("N");
				}
				else
				{
					meta.SessionId = NormalizeSessionId(meta.SessionId);
				}
				int stableStringHash = GetStableStringHash(meta.SessionId);
				int value = BuildRecoveryMetaCheck(meta.Version, meta.SlotId, meta.BaselineUtcTicks, meta.LastAutoBackupUtcTicks, meta.LastExitBackupUtcTicks, stableStringHash);
				binaryWriter.Write(meta.Version);
				binaryWriter.Write(meta.SlotId);
				binaryWriter.Write(meta.BaselineUtcTicks);
				binaryWriter.Write(meta.LastAutoBackupUtcTicks);
				binaryWriter.Write(meta.LastExitBackupUtcTicks);
				binaryWriter.Write(stableStringHash);
				binaryWriter.Write(value);
				binaryWriter.Write(Guid.Parse(meta.SessionId).ToByteArray());
				binaryWriter.Flush();
				fileStream.Flush();
			}
			ReplaceFileKeepingBackup(text, path, backupPath);
		}
		catch (Exception ex)
		{
			LogUtil.Warn("保存恢复元数据失败：" + ex.Message);
		}
	}

	private static int BuildRecoveryMetaCheck(int version, int slotId, long baselineTicks, long autoTicks, long exitTicks, int sessionHash)
	{
		return ((((((((((0x6EDED82D ^ version) * 397) ^ slotId) * 397) ^ baselineTicks.GetHashCode()) * 397) ^ autoTicks.GetHashCode()) * 397) ^ exitTicks.GetHashCode()) * 397) ^ sessionHash;
	}

	private static int GetStableStringHash(string value)
	{
		int num = 23;
		if (!string.IsNullOrEmpty(value))
		{
			for (int i = 0; i < value.Length; i++)
			{
				num = num * 31 + value[i];
			}
		}
		return num;
	}

	private static bool IsValidSessionId(string sessionId)
	{
		Guid result;
		if (!string.IsNullOrEmpty(sessionId))
		{
			return Guid.TryParse(sessionId, out result);
		}
		return false;
	}

	private static string NormalizeSessionId(string sessionId)
	{
		if (!Guid.TryParse(sessionId, out var result))
		{
			return "";
		}
		return result.ToString("N");
	}

	private static IEnumerable<int> GetAllSlots()
	{
		HashSet<int> yielded = new HashSet<int>();
		SaveManifest saveManifest = LoadSaveManifest();
		foreach (SaveManifestSlotEntry slot in saveManifest.Slots)
		{
			if (slot.SlotId >= 0 && yielded.Add(slot.SlotId))
			{
				yield return slot.SlotId;
			}
		}
		foreach (string item in EnumerateSlotCandidateFiles())
		{
			if (TryParseSlotIdFromPath(item, out var id) && yielded.Add(id))
			{
				yield return id;
			}
		}
	}

	private static bool HasSaveSlot(int slotId)
	{
		if (!GetSlotCandidatePaths(slotId, SaveBackupKind.EntryBaseline).Any((string path) => GetRecoverablePaths(path).Any()) && !GetSlotCandidatePaths(slotId, SaveBackupKind.AutoBackup).Any((string path) => GetRecoverablePaths(path).Any()))
		{
			return GetSlotCandidatePaths(slotId, SaveBackupKind.ExitBackup).Any((string path) => GetRecoverablePaths(path).Any());
		}
		return true;
	}

	private static bool HasRecoverableManifestEntryPath(SaveManifestSlotEntry entry)
	{
		if (entry == null)
		{
			return false;
		}
		if (!HasRecoverableManifestPath(entry.BaselinePath) && !HasRecoverableManifestPath(entry.AutoPath))
		{
			return HasRecoverableManifestPath(entry.ExitPath);
		}
		return true;
	}

	private static bool HasRecoverableManifestPath(string manifestPath)
	{
		if (string.IsNullOrEmpty(manifestPath))
		{
			return false;
		}
		string text = ResolveManifestPath(manifestPath, SaveRoot);
		if (!string.IsNullOrEmpty(text))
		{
			return GetRecoverablePaths(text).Any();
		}
		return false;
	}

	private static IEnumerable<string> EnumerateSlotCandidateFiles()
	{
		foreach (string root in EnumerateSaveRoots())
		{
			if (!Directory.Exists(root))
			{
				continue;
			}
			foreach (string extension in EnumerateSaveExtensions())
			{
				foreach (string item in SafeEnumerateFiles(root, "slot_*" + extension))
				{
					yield return item;
				}
				foreach (string item2 in SafeEnumerateFiles(root, "slot_*" + extension + ".bak"))
				{
					yield return item2;
				}
				foreach (string item3 in SafeEnumerateFiles(root, "slot_*" + extension + ".replacebak.*"))
				{
					yield return item3;
				}
			}
		}
	}

	private static bool TryParseSlotIdFromPath(string path, out int id)
	{
		if (TryParseSlotFileIdentity(path, out id, out var _))
		{
			return true;
		}
		return false;
	}

	private static bool IsSlotFileIdentityExpected(string path, int expectedSlotId, SaveBackupKind expectedKind)
	{
		if (TryParseSlotFileIdentity(path, out var id, out var backupKind) && id == expectedSlotId)
		{
			return backupKind == expectedKind;
		}
		return false;
	}

	private static bool TryParseSlotFileIdentity(string path, out int id, out SaveBackupKind backupKind)
	{
		id = -1;
		backupKind = SaveBackupKind.EntryBaseline;
		string text = Path.GetFileName(path);
		int num = text.IndexOf(".replacebak.", StringComparison.Ordinal);
		if (num >= 0)
		{
			text = text.Substring(0, num);
		}
		if (text.EndsWith(".bak", StringComparison.Ordinal))
		{
			text = text.Substring(0, text.Length - ".bak".Length);
		}
		string[] knownSaveExtensions = KnownSaveExtensions;
		foreach (string text2 in knownSaveExtensions)
		{
			if (text.EndsWith(text2, StringComparison.OrdinalIgnoreCase))
			{
				text = text.Substring(0, text.Length - text2.Length);
				break;
			}
		}
		return TryParseSlotIdAndBackupKindFromFileName(text, out id, out backupKind);
	}

	private static bool TryParseSlotIdFromFileName(string name, out int id)
	{
		SaveBackupKind backupKind;
		return TryParseSlotIdAndBackupKindFromFileName(name, out id, out backupKind);
	}

	private static bool TryParseSlotIdAndBackupKindFromFileName(string name, out int id, out SaveBackupKind backupKind)
	{
		id = -1;
		backupKind = SaveBackupKind.EntryBaseline;
		if (string.IsNullOrEmpty(name) || !name.StartsWith("slot_", StringComparison.Ordinal))
		{
			return false;
		}
		string text = name.Substring("slot_".Length);
		int num = text.IndexOf('_');
		string text2 = "";
		if (num >= 0)
		{
			text2 = text.Substring(num);
			text = text.Substring(0, num);
		}
		if (!int.TryParse(text, out id))
		{
			return false;
		}
		if (string.IsNullOrEmpty(text2))
		{
			backupKind = SaveBackupKind.EntryBaseline;
			return true;
		}
		if (text2 == "_auto")
		{
			backupKind = SaveBackupKind.AutoBackup;
			return true;
		}
		if (text2 == "_exit")
		{
			backupKind = SaveBackupKind.ExitBackup;
			return true;
		}
		return false;
	}

	private static SaveData CreateInitData(string playerName, int typeId)
	{
		SaveData saveData = SaveData.CreateNew();
		saveData.PlayerData.PlayerName = playerName;
		saveData.PlayerData.PlayerType = typeId;
		return saveData;
	}

	private static bool SaveToSlot(int slotId, SaveData data)
	{
		EnsureFolder();
		if (data == null)
		{
			LogUtil.Error($"保存 slot_{slotId} 失败：data 为空");
			return false;
		}
		data.GameVersion = Application.version;
		try
		{
			AtomicSave(GetSlotPath(slotId), data);
			return true;
		}
		catch (Exception arg)
		{
			LogUtil.Error($"保存 slot_{slotId} 失败：{arg}");
			return false;
		}
	}

	private static void BuildRuntimeFromSaveData(SaveData data)
	{
		if (data == null)
		{
			RuntimeData = null;
			return;
		}
		data.PostLoadFix();
		RuntimeData = data;
	}

	private static void EnsureFolder()
	{
		if (!Directory.Exists(SaveRoot))
		{
			Directory.CreateDirectory(SaveRoot);
		}
	}

	private static string GetSlotPath(int slotId)
	{
		return Path.Combine(SaveRoot, string.Format("slot_{0}{1}", slotId, ".sav"));
	}

	private static string GetSlotPath(int slotId, string extension)
	{
		return Path.Combine(SaveRoot, $"slot_{slotId}{extension}");
	}

	private static string GetSlotBackupPath(int slotId, SaveBackupKind backupKind)
	{
		return GetSlotBackupPath(slotId, backupKind, ".sav");
	}

	private static string GetSlotBackupPath(int slotId, SaveBackupKind backupKind, string extension)
	{
		return backupKind switch
		{
			SaveBackupKind.AutoBackup => Path.Combine(SaveRoot, string.Format("slot_{0}{1}{2}", slotId, "_auto", extension)), 
			SaveBackupKind.ExitBackup => Path.Combine(SaveRoot, string.Format("slot_{0}{1}{2}", slotId, "_exit", extension)), 
			_ => GetSlotPath(slotId, extension), 
		};
	}

	private static IEnumerable<string> GetSlotBackupPathVariants(int slotId, SaveBackupKind backupKind)
	{
		foreach (string extension in EnumerateSaveExtensions())
		{
			yield return GetSlotBackupPath(slotId, backupKind, extension);
			if (Directory.Exists(LegacySaveRoot))
			{
				yield return GetSlotBackupPath(LegacySaveRoot, slotId, backupKind, extension);
			}
		}
	}

	private static IEnumerable<string> GetGlobalPathVariants()
	{
		foreach (string extension in EnumerateSaveExtensions())
		{
			yield return Path.Combine(SaveRoot, "global" + extension);
			if (Directory.Exists(LegacySaveRoot))
			{
				yield return Path.Combine(LegacySaveRoot, "global" + extension);
			}
		}
	}

	private static IEnumerable<string> GetSlotCandidatePaths(int slotId, SaveBackupKind backupKind)
	{
		string text = LoadSaveManifest().Slots.FirstOrDefault((SaveManifestSlotEntry s) => s != null && s.SlotId == slotId)?.GetPath(backupKind);
		if (!string.IsNullOrEmpty(text))
		{
			string text2 = ResolveManifestPath(text, SaveRoot);
			if (IsSlotFileIdentityExpected(text2, slotId, backupKind))
			{
				yield return text2;
			}
		}
		foreach (string slotBackupPathVariant in GetSlotBackupPathVariants(slotId, backupKind))
		{
			yield return slotBackupPathVariant;
		}
	}

	private static IEnumerable<string> EnumerateSaveExtensions()
	{
		yield return ".sav";
		string[] knownSaveExtensions = KnownSaveExtensions;
		string[] array = knownSaveExtensions;
		foreach (string text in array)
		{
			if (!string.Equals(text, ".sav", StringComparison.OrdinalIgnoreCase))
			{
				yield return text;
			}
		}
	}

	private static IEnumerable<string> EnumerateSaveRoots()
	{
		yield return SaveRoot;
		if (Directory.Exists(LegacySaveRoot))
		{
			yield return LegacySaveRoot;
		}
	}

	private static string GetManifestPath(string saveRootPath)
	{
		return Path.Combine(saveRootPath, "save_manifest.meta");
	}

	private static string GetManifestBackupPath(string saveRootPath)
	{
		return GetManifestPath(saveRootPath) + ".bak";
	}

	private static string ToManifestRelativePath(string path, string saveRootPath)
	{
		try
		{
			string text = Path.GetFullPath(saveRootPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
			string fullPath = Path.GetFullPath(path);
			if (fullPath.StartsWith(text, StringComparison.OrdinalIgnoreCase))
			{
				return fullPath.Substring(text.Length).Replace(Path.DirectorySeparatorChar, '/');
			}
		}
		catch
		{
		}
		return Path.GetFileName(path);
	}

	private static string ResolveManifestPath(string manifestPath, string saveRootPath)
	{
		if (string.IsNullOrEmpty(manifestPath))
		{
			return "";
		}
		try
		{
			string value = Path.GetFullPath(saveRootPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
			string fullPath = Path.GetFullPath(Path.IsPathRooted(manifestPath) ? manifestPath : Path.Combine(saveRootPath, manifestPath.Replace('/', Path.DirectorySeparatorChar)));
			return fullPath.StartsWith(value, StringComparison.OrdinalIgnoreCase) ? fullPath : "";
		}
		catch
		{
			return "";
		}
	}

	private static string GetRecoveryMetaPath(int slotId)
	{
		return Path.Combine(SaveRoot, string.Format("slot_{0}{1}", slotId, "_recovery.meta"));
	}

	private static string GetRecoveryMetaBackupPath(int slotId)
	{
		return Path.Combine(SaveRoot, string.Format("slot_{0}{1}", slotId, "_recovery.meta.bak"));
	}

	private static string GetSlotBackupPath(string root, int slotId, SaveBackupKind backupKind, string extension)
	{
		return backupKind switch
		{
			SaveBackupKind.AutoBackup => Path.Combine(root, string.Format("slot_{0}{1}{2}", slotId, "_auto", extension)), 
			SaveBackupKind.ExitBackup => Path.Combine(root, string.Format("slot_{0}{1}{2}", slotId, "_exit", extension)), 
			_ => Path.Combine(root, $"slot_{slotId}{extension}"), 
		};
	}

	private static void AtomicSave<T>(string finalPath, T data)
	{
		Directory.CreateDirectory(Path.GetDirectoryName(finalPath) ?? string.Empty);
		string path = Path.GetDirectoryName(finalPath) ?? string.Empty;
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(finalPath);
		string extension = Path.GetExtension(finalPath);
		string text = Path.Combine(path, fileNameWithoutExtension + ".tmp" + extension);
		string backupPath = finalPath + ".bak";
		try
		{
			if (File.Exists(text))
			{
				File.Delete(text);
			}
			DataUtil.Save(text, data);
			if (DataUtil.Load<T>(text) == null)
			{
				throw new IOException("临时存档校验失败：" + text);
			}
			ReplaceFileKeepingBackup(text, finalPath, backupPath);
		}
		catch
		{
			DeleteSlotFileIfExists(text);
			throw;
		}
	}

	private static bool TryLoadSaveFile<T>(string path, out T data)
	{
		data = default(T);
		if (string.IsNullOrEmpty(path) || !File.Exists(path))
		{
			return false;
		}
		if (TryLoadWith(path, DataUtil.Load<T>, out data))
		{
			return true;
		}
		if (string.Equals(Path.GetExtension(path), ".json", StringComparison.OrdinalIgnoreCase))
		{
			return false;
		}
		return TryLoadWith(path, DataUtil.LoadPlain<T>, out data);
	}

	private static bool TryLoadWith<T>(string path, Func<string, T> loader, out T data)
	{
		data = default(T);
		try
		{
			data = loader(path);
			return !EqualityComparer<T>.Default.Equals(data, default(T));
		}
		catch (Exception ex)
		{
			LogUtil.Warn("读取存档候选失败：" + path + ", reason = " + ex.Message);
			return false;
		}
	}

	private static void ReplaceFileKeepingBackup(string tempPath, string finalPath, string backupPath)
	{
		if (File.Exists(finalPath))
		{
			string uniqueReplaceBackupPath = GetUniqueReplaceBackupPath(finalPath);
			File.Replace(tempPath, finalPath, uniqueReplaceBackupPath);
			CopyFileBestEffort(uniqueReplaceBackupPath, backupPath);
			PruneReplaceBackups(finalPath, 3);
		}
		else
		{
			File.Move(tempPath, finalPath);
			CopyFileBestEffort(finalPath, backupPath);
		}
	}

	private static IEnumerable<string> GetRecoverablePaths(string finalPath)
	{
		if (string.IsNullOrEmpty(finalPath))
		{
			yield break;
		}
		if (File.Exists(finalPath))
		{
			yield return finalPath;
		}
		string text = finalPath + ".bak";
		if (File.Exists(text))
		{
			yield return text;
		}
		foreach (string replaceBackupPath in GetReplaceBackupPaths(finalPath))
		{
			yield return replaceBackupPath;
		}
	}

	private static string GetUniqueReplaceBackupPath(string finalPath)
	{
		return $"{finalPath}.replacebak.{DateTime.UtcNow.Ticks}";
	}

	private static IEnumerable<string> GetReplaceBackupPaths(string finalPath)
	{
		string text = Path.GetDirectoryName(finalPath) ?? string.Empty;
		string fileName = Path.GetFileName(finalPath);
		if (!Directory.Exists(text))
		{
			return Enumerable.Empty<string>();
		}
		return SafeEnumerateFiles(text, fileName + ".replacebak.*").OrderByDescending(SafeGetLastWriteTimeUtc);
	}

	private static DateTime SafeGetLastWriteTimeUtc(string path)
	{
		try
		{
			return (string.IsNullOrEmpty(path) || !File.Exists(path)) ? DateTime.MinValue : File.GetLastWriteTimeUtc(path);
		}
		catch (Exception ex)
		{
			LogUtil.Warn("读取存档文件时间失败：" + path + ", reason = " + ex.Message);
			return DateTime.MinValue;
		}
	}

	private static IEnumerable<string> SafeEnumerateFiles(string directory, string searchPattern)
	{
		try
		{
			if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory))
			{
				return Enumerable.Empty<string>();
			}
			return Directory.GetFiles(directory, searchPattern);
		}
		catch (Exception ex)
		{
			LogUtil.Warn("枚举存档文件失败：" + directory + ", pattern = " + searchPattern + ", reason = " + ex.Message);
			return Enumerable.Empty<string>();
		}
	}

	private static IEnumerable<string> EnumerateSaveFilesForDelete()
	{
		if (!Directory.Exists(SaveRoot))
		{
			yield break;
		}
		foreach (string item in SafeEnumerateFiles(SaveRoot, "slot_*"))
		{
			yield return item;
		}
		foreach (string item2 in SafeEnumerateFiles(SaveRoot, "global*"))
		{
			yield return item2;
		}
		foreach (string item3 in SafeEnumerateFiles(SaveRoot, "save_manifest.meta*"))
		{
			yield return item3;
		}
		foreach (string item4 in SafeEnumerateFiles(SaveRoot, "last_save_id.sav*"))
		{
			yield return item4;
		}
	}

	private static void PruneReplaceBackups(string finalPath, int keepCount)
	{
		foreach (string item in GetReplaceBackupPaths(finalPath).Skip(Math.Max(0, keepCount)))
		{
			DeleteSlotFileIfExists(item);
		}
	}

	private static void CopyFileBestEffort(string sourcePath, string destinationPath)
	{
		try
		{
			if (File.Exists(sourcePath))
			{
				File.Copy(sourcePath, destinationPath, overwrite: true);
			}
		}
		catch (Exception ex)
		{
			LogUtil.Warn("复制存档备份失败：" + sourcePath + " -> " + destinationPath + ", reason = " + ex.Message);
		}
	}

	private static void DeleteSlotFileIfExists(string path)
	{
		try
		{
			if (!string.IsNullOrEmpty(path) && File.Exists(path))
			{
				File.Delete(path);
			}
		}
		catch (Exception ex)
		{
			LogUtil.Warn("删除存档相关文件失败：" + path + ", reason = " + ex.Message);
		}
	}

	private static void DeleteRecoverableFiles(string finalPath)
	{
		foreach (string item in GetRecoverablePaths(finalPath).ToList())
		{
			DeleteSlotFileIfExists(item);
		}
		DeleteSlotFileIfExists(finalPath);
		DeleteSlotFileIfExists(finalPath + ".bak");
	}

	private static int AllocateNewSlotId()
	{
		int num = 1;
		foreach (int allSlot in GetAllSlots())
		{
			if (allSlot >= num && HasSaveSlot(allSlot))
			{
				num = allSlot + 1;
			}
		}
		return num;
	}
}
