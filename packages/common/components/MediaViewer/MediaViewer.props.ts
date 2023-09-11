import {
  ContextMenuAction,
  IFile,
  NumberOrString,
  OmitSecondArg,
  PlaylistType,
  TranslationType,
} from "./types";
export interface MediaViewerProps {
  t: TranslationType;

  userAccess: boolean;
  currentFileId: NumberOrString;

  visible: boolean;

  extsMediaPreviewed: string[];
  extsImagePreviewed: string[];

  someDialogIsOpen: boolean;

  deleteDialogVisible: boolean;
  errorLabel: string;
  isPreviewFile: boolean;

  files: IFile[];

  playlist: PlaylistType[];

  setBufferSelection: Function;
  archiveRoomsId: number;

  playlistPos: number;

  getIcon: (size: number, ext: string, ...arg: any) => string;

  onClose: VoidFunction;
  onError?: VoidFunction;
  onEmptyPlaylistError: VoidFunction;
  onChangeUrl: (id: NumberOrString) => void;
  onShowInfoPanel: OmitSecondArg<ContextMenuAction>;
  onDelete: (id: NumberOrString) => void;
  onDownload: (id: NumberOrString) => void;

  onClickDownloadAs: VoidFunction;
  onMoveAction: VoidFunction;
  onCopyAction: VoidFunction;
  onClickRename: OmitSecondArg<ContextMenuAction>;
  onDuplicate: ContextMenuAction;
  onClickDelete: ContextMenuAction;
  onClickDownload: ContextMenuAction;
  onClickLinkEdit: OmitSecondArg<ContextMenuAction>;
  onPreviewClick: OmitSecondArg<ContextMenuAction>;
  onCopyLink: ContextMenuAction;

  nextMedia: VoidFunction;
  prevMedia: VoidFunction;
}
