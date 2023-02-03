

export type NumberOrString = number | string;

export type PlaylistType = {
    id: number
    canShare: boolean;
    fileExst: string;
    fileId: number;
    fileStatus: number;
    src: string;
    title: string;
}


export type CreatedType = {
    id: string;
    avatarSmall: string;
    displayName: string;
    hasAvatar: boolean;
    profileUrl: string;
}



export type SecurityType = {
    Comment: boolean;
    Copy: boolean;
    CustomFilter: boolean;
    Delete: boolean;
    Duplicate: boolean;
    Edit: boolean;
    EditHistory: boolean;
    FillForms: boolean;
    Lock: boolean;
    Move: boolean;
    Read: boolean;
    ReadHistory: boolean;
    Rename: boolean;
    Review: boolean;
}

export type ViewAccessabilityType = {
    CoAuhtoring: boolean;
    Convert: boolean;
    ImageView: boolean;
    MediaView: boolean;
    WebComment: boolean;
    WebCustomFilterEditing: boolean;
    WebEdit: boolean;
    WebRestrictedEditing: boolean;
    WebReview: boolean;
    WebView: boolean;
}



export interface IFile {
    id: number;
    access: number;
    canShare: boolean;
    comment: string;
    contentLength: string;
    created: string;
    createdBy: CreatedType;
    denyDownload: boolean;
    denySharing: boolean;
    fileExst: string;
    fileStatus: number;
    fileType: number;
    folderId: number;
    pureContentLength: number;
    rootFolderId: number;
    rootFolderType: number;
    security: SecurityType;
    shared: boolean;
    thumbnailStatus: number;
    title: string;
    updated: string;
    updatedBy: CreatedType;
    version: number;
    versionGroup: number;
    viewAccessability: ViewAccessabilityType;
    viewUrl: string;
    webUrl: string;
}



export interface MediaViewerProps {
    t: Function;
    userAccess: boolean;
    currentFileId: NumberOrString;

    visible: boolean;

    extsMediaPreviewed: string;
    extsImagePreviewed: string;

    deleteDialogVisible: boolean
    errorLabel: string;
    isPreviewFile: boolean;

    files: IFile[]

    getPlaylist: () => PlaylistType[]

    setBufferSelection: Function;

    onClose?: VoidFunction;
    onError?: VoidFunction;
    onEmptyPlaylistError: VoidFunction
    onDelete?: (id: NumberOrString) => void;
    onDownload?: (id: NumberOrString) => void;
    onChangeUrl?: (id: NumberOrString) => void;

    onClickDownload

}
