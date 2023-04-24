import { IFile, NumberOrString, PlaylistType, TranslationType } from "./types";
export interface MediaViewerProps {
  t: TranslationType;

  userAccess: boolean;
  currentFileId: NumberOrString;

  visible: boolean;

  extsMediaPreviewed: string[];
  extsImagePreviewed: string[];

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
  onDelete: (id: NumberOrString) => void;
  onDownload: (id: NumberOrString) => void;
  onChangeUrl: (id: NumberOrString) => void;

  onMoveAction: VoidFunction;
  onCopyAction: VoidFunction;
  onClickRename: (file: IFile) => void;
  onShowInfoPanel: (file: IFile) => void;
  onDuplicate: (file: IFile, t: TranslationType) => void;
  onClickDelete: (file: IFile, t: TranslationType) => void;
  onClickDownload: (file: IFile, t: TranslationType) => void;

  nextMedia: VoidFunction;
  prevMedia: VoidFunction;
}
