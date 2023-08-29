import EmptyScreenFilterAltSvgUrl from "PUBLIC_DIR/images/empty_screen_filter_alt.svg?url";
import EmptyScreenFilterAltDarkSvgUrl from "PUBLIC_DIR/images/empty_screen_filter_alt_dark.svg?url";
import ClearEmptyFilterSvgUrl from "PUBLIC_DIR/images/clear.empty.filter.svg?url";
import React from "react";
import { useNavigate, useLocation } from "react-router-dom";
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

  linkStyles,
  isRooms,
  isArchiveFolder,
  isRoomsFolder,
  setClearSearch,
  theme,
  isPublicRoom,
  publicRoomKey,
}) => {
  const navigate = useNavigate();
  const location = useLocation();

  const subheadingText = t("EmptyFilterSubheadingText");
  const descriptionText = isRooms
    ? t("Common:SearchEmptyRoomsDescription")
    : t("EmptyFilterDescriptionText");

  const onResetFilter = () => {
    setIsLoading(true);

    if (isArchiveFolder) {
      setClearSearch(true);
      return;
    }

    if (isRoomsFolder) {
      const newFilter = RoomsFilter.getDefault();

      navigate(`${location.pathname}?${newFilter.toUrlParams()}`);
    } else {
      const newFilter = FilesFilter.getDefault();

      newFilter.folder = selectedFolderId;

      if (isPublicRoom) {
        navigate(
          `${location.pathname}?key=${publicRoomKey}&${newFilter.toUrlParams()}`
        );
      } else {
        navigate(`${location.pathname}?${newFilter.toUrlParams()}`);
      }
    }
  };

  const buttons = (
    <div className="empty-folder_container-links">
      <IconButton
        className="empty-folder_container-icon"
        size="12"
        onClick={onResetFilter}
        iconName={ClearEmptyFilterSvgUrl}
        isFill
      />
      <Link onClick={onResetFilter} {...linkStyles}>
        {t("Common:ClearFilter")}
      </Link>
    </div>
  );

  const imageSrc = theme.isBase
    ? EmptyScreenFilterAltSvgUrl
    : EmptyScreenFilterAltDarkSvgUrl;

  return (
    <EmptyContainer
      headerText={t("Common:NotFoundTitle")}
      descriptionText={descriptionText}
      imageSrc={imageSrc}
      buttons={buttons}
    />
  );
};

export default inject(
  ({
    auth,
    filesStore,
    selectedFolderStore,
    treeFoldersStore,
    clientLoadingStore,
    publicRoomStore,
  }) => {
    const { isRoomsFolder, isArchiveFolder } = treeFoldersStore;

    const isRooms = isRoomsFolder || isArchiveFolder;
    const { isPublicRoom, publicRoomKey } = publicRoomStore;

    return {
      selectedFolderId: selectedFolderStore.id,
      setIsLoading: clientLoadingStore.setIsSectionBodyLoading,
      isRooms,
      isArchiveFolder,
      isRoomsFolder,
      setClearSearch: filesStore.setClearSearch,
      theme: auth.settingsStore.theme,

      isPublicRoom,
      publicRoomKey,
    };
  }
)(withTranslation(["Files", "Common"])(observer(EmptyFilterContainer)));
