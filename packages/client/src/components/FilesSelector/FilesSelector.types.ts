export type Security = {
  Copy: boolean;
  CopyTo: boolean;
  Create: boolean;
  Delete: boolean;
  Duplicate: boolean;
  EditAccess: boolean;
  EditRoom: boolean;
  Move: boolean;
  MoveTo: boolean;
  Mute: boolean;
  Pin: boolean;
  Read: boolean;
  Rename: boolean;
};

export type Item = {
  id: number | string;
  parentId: number | string;
  rootFolderType: number | string;
  title: string;
  label: string;
  filesCount?: number;
  foldersCount?: number;
  avatar?: string;
  icon?: string;
  isFolder: boolean;
  isDisabled?: boolean;
  security: Security;
  roomType: number;
};

export type BreadCrumb = {
  label: string;
  id: number | string;
  isRoom: boolean;
};

type setItems = (value: Item[] | null) => Item[];

export type useLoadersHelperProps = {
  items: Item[] | null;
};

export type setItemsCallback = (value: Item[] | null) => Item[] | null;
export type setBreadCrumbsCallback = (
  value: BreadCrumb[] | []
) => BreadCrumb[] | [];
export type setTotalCallback = (value: number) => number;

export type useSocketHelperProps = {
  socketHelper: any;
  socketSubscribersId: Set<string>;
  setItems: (callback: setItemsCallback) => void;
  setBreadCrumbs: (callback: setBreadCrumbsCallback) => void;
  setTotal: (callback: setTotalCallback) => void;
  disabledItems: string[] | number[];
  filterParam?: string;
};

export type useRootHelperProps = {
  setBreadCrumbs: (items: BreadCrumb[]) => void;
  setIsBreadCrumbsLoading: (value: boolean) => void;
  setTotal: (value: number) => void;
  setItems: (items: Item[] | setItems) => void;
  treeFolders?: Item[];
  setIsNextPageLoading: (value: boolean) => void;
  setHasNextPage: (value: boolean) => void;
};

export type useRoomsHelperProps = {
  setBreadCrumbs: (items: BreadCrumb[]) => void;
  setIsBreadCrumbsLoading: (value: boolean) => void;
  setIsNextPageLoading: (value: boolean) => void;
  setHasNextPage: (value: boolean) => void;
  setTotal: (value: number) => void;
  setItems: (items: Item[] | setItems) => void;
  isFirstLoad: boolean;
  setIsRoot: (value: boolean) => void;
  searchValue?: string;
  isRoomsOnly: boolean;
  onSetBaseFolderPath?: (value: number | string | undefined) => void;
};

export type useFilesHelpersProps = {
  setBreadCrumbs: (items: BreadCrumb[]) => void;
  setIsBreadCrumbsLoading: (value: boolean) => void;
  setIsNextPageLoading: (value: boolean) => void;
  setHasNextPage: (value: boolean) => void;
  setTotal: (value: number) => void;
  setItems: (items: Item[] | setItems) => void;
  isFirstLoad: boolean;
  selectedItemId: string | number | undefined;
  setIsRoot: (value: boolean) => void;
  searchValue?: string;
  disabledItems: string[] | number[];
  setSelectedItemSecurity: (value: Security) => void;
  isThirdParty: boolean;
  onSelectTreeNode?: (treeNode: any) => void;
  setSelectedTreeNode: (treeNode: any) => void;
  filterParam?: string;
  getRootData?: () => Promise<void>;
  onSetBaseFolderPath?: (value: number | string | undefined) => void;
  isRoomsOnly: boolean;
  rootThirdPartyId?: string;
  getRoomList?: (
    startIndex: number,
    isInit?: boolean,
    search?: string | null,
    isErrorPath?: boolean
  ) => void;
  t: any;
};

export type FilesSelectorProps = {
  isPanelVisible: boolean;
  // withoutImmediatelyClose: boolean;
  isThirdParty: boolean;
  rootThirdPartyId?: string;
  isRoomsOnly: boolean;
  isEditorDialog: boolean;
  setMoveToPublicRoomVisible: (visible: boolean, operationData: object) => void;

  onClose?: () => void;

  isMove?: boolean;
  isCopy?: boolean;
  isRestoreAll?: boolean;

  filterParam?: string;

  currentFolderId?: number;
  fromFolderId?: number;
  parentId?: number;
  rootFolderType?: number;

  treeFolders?: Item[];

  theme: any;

  selection: any[];
  disabledItems: string[] | number[];
  isFolderActions?: boolean;
  setMoveToPanelVisible: (value: boolean) => void;
  setCopyPanelVisible: (value: boolean) => void;
  setRestoreAllPanelVisible: (value: boolean) => void;
  setIsFolderActions: (value: boolean) => void;
  setMovingInProgress: (value: boolean) => void;
  setConflictDialogData: (conflicts: any, operationData: any) => void;
  itemOperationToFolder: (operationData: any) => Promise<void>;
  clearActiveOperations: (
    folderIds: string[] | number[],
    fileIds: string[] | number[]
  ) => void;
  checkFileConflicts: (
    selectedItemId: string | number | undefined,
    folderIds: string[] | number[],
    fileIds: string[] | number[]
  ) => Promise<any>;

  onSetBaseFolderPath?: (value: number | string | undefined) => void;
  onSetNewFolderPath?: (value: number | string | undefined) => void;
  onSelectFolder?: (value: number | string | undefined) => void;
  onSelectTreeNode?: (treeNode: any) => void;
  onSave?: (
    e: any,
    folderId: string | number,
    fileTitle: string,
    openNewTab: boolean
  ) => void;
  onSelectFile?: (fileInfo: {
    id: string | number;
    title: string;
    path?: string[];
  }) => void;

  withFooterInput?: boolean;
  withFooterCheckbox?: boolean;
  footerInputHeader?: string;
  currentFooterInputValue?: string;
  footerCheckboxLabel?: string;

  descriptionText?: string;
  setSelectedItems: () => void;

  includeFolder?: boolean;

  socketHelper: any;
  socketSubscribersId: Set<string>;
};
