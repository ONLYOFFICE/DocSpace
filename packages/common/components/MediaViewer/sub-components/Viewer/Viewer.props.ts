import { ContextMenuModel } from "../../types";

interface ViewerProps {
  userAccess: boolean;
  visible: boolean;
  title: string;
  images: [{ src: string; alt: string }];
  inactive: boolean;
  playlist: number;
  playlistPos: number;
  isFavorite: boolean;
  isImage: boolean;
  isAudio: boolean;
  isVideo: boolean;
  isPreviewFile: boolean;
  archiveRoom: boolean;

  errorTitle: string;
  headerIcon: string;
  audioIcon: string;

  onClose: VoidFunction;
  onPrevClick: VoidFunction;
  onNextClick: VoidFunction;
  onDeleteClick: VoidFunction;
  onDownloadClick: VoidFunction;
  onSetSelectionFile: VoidFunction;
  contextModel: () => ContextMenuModel[];
}

export default ViewerProps;
