import React from "react";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import EmptyContainer from "./EmptyContainer";
import FilesFilter from "@docspace/common/api/files/filter";
import RoomsFilter from "@docspace/common/api/rooms/filter";
import Link from "@docspace/components/link";
import IconButton from "@docspace/components/icon-button";
import toastr from "@docspace/components/toast/toastr";

const EmptyFilterContainer = ({
  t,
  selectedFolderId,
  setIsLoading,
  fetchFiles,
  fetchRooms,
  linkStyles,
  isRooms,
}) => {
  const subheadingText = t("EmptyFilterSubheadingText");
  const descriptionText = t("EmptyFilterDescriptionText");

  const onResetFilter = () => {
    setIsLoading(true);
    if (isRooms) {
      const newFilter = RoomsFilter.getDefault();
      fetchRooms(selectedFolderId, newFilter)
        .catch((err) => toastr.error(err))
        .finally(() => setIsLoading(false));
    } else {
      const newFilter = FilesFilter.getDefault();
      fetchFiles(selectedFolderId, newFilter)
        .catch((err) => toastr.error(err))
        .finally(() => setIsLoading(false));
    }
  };

  const buttons = (
    <div className="empty-folder_container-links">
      <IconButton
        className="empty-folder_container-icon"
        size="12"
        onClick={onResetFilter}
        iconName="/static/images/clear.empty.filter.svg"
        isFill
      />
      <Link onClick={onResetFilter} {...linkStyles}>
        {t("Common:ClearFilter")}
      </Link>
    </div>
  );

  return (
    <EmptyContainer
      headerText={t("Common:NotFoundTitle")}
      descriptionText={descriptionText}
      imageSrc="images/empty_screen_filter_alt.svg"
      buttons={buttons}
    />
  );
};

export default inject(
  ({ filesStore, selectedFolderStore, treeFoldersStore }) => {
    const { isRoomsFolder, isArchiveFolder } = treeFoldersStore;

    const isRooms = isRoomsFolder || isArchiveFolder;

    return {
      fetchFiles: filesStore.fetchFiles,
      fetchRooms: filesStore.fetchRooms,
      selectedFolderId: selectedFolderStore.id,
      setIsLoading: filesStore.setIsLoading,
      isRooms,
    };
  }
)(withTranslation(["Files", "Common"])(observer(EmptyFilterContainer)));
