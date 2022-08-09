class PropsHandler {
  constructor(props) {
    this.props = props;
  }

  defaultProps() {
    return {
      t: this.props.t,
      selectedItems: this.props.selectedItems,
      personal: this.props.personal,
      culture: this.props.culture,
      isRootFolder: this.props.isRootFolder,
      isRecycleBinFolder: this.props.isRecycleBinFolder,
      isRecentFolder: this.props.isRecentFolder,
      isFavoritesFolder: this.props.isFavoritesFolder,
    };
  }

  filesProps() {
    return {
      selectedFolder: this.props.selectedFolder,
      getFolderInfo: this.props.getFolderInfo,
      getIcon: this.props.getIcon,
      getFolderIcon: this.props.getFolderIcon,
      getShareUsers: this.props.getShareUsers,
      onSelectItem: this.props.onSelectItem,
      setSharingPanelVisible: this.props.setSharingPanelVisible,
      createThumbnail: this.props.createThumbnail,
    };
  }

  galleryProps() {
    return {
      gallerySelected: this.props.gallerySelected,
      personal: this.props.personal,
      culture: this.props.culture,
    };
  }
}

export default PropsHandler;
