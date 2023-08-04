import EmailReactSvgUrl from "PUBLIC_DIR/images/email.react.svg?url";
import SecurityReactSvgUrl from "PUBLIC_DIR/images/security.react.svg?url";
import ImageReactSvgUrl from "PUBLIC_DIR/images/image.react.svg?url";
import CatalogTrashReactSvgUrl from "PUBLIC_DIR/images/catalog.trash.react.svg?url";
import ArrowPathReactSvgUrl from "PUBLIC_DIR/images/arrow.path.react.svg?url";
import VerticalDotsReactSvgUrl from "PUBLIC_DIR/images/vertical-dots.react.svg?url";
import React, { useState } from "react";
import { withTranslation } from "react-i18next";
import { useNavigate, useLocation } from "react-router-dom";
import { inject, observer } from "mobx-react";

import IconButton from "@docspace/components/icon-button";
import ContextMenuButton from "@docspace/components/context-menu-button";
import Headline from "@docspace/common/components/Headline";
import Loaders from "@docspace/common/components/Loaders";
import { DeleteSelfProfileDialog } from "SRC_DIR/components/dialogs";
import { DeleteOwnerProfileDialog } from "SRC_DIR/components/dialogs";
import { combineUrl } from "@docspace/common/utils";
import config from "PACKAGE_FILE";

import { StyledHeader } from "./StyledHeader";
import RoomsFilter from "@docspace/common/api/rooms/filter";
import { RoomSearchArea } from "@docspace/common/constants";

const Header = (props) => {
  const {
    t,

    isAdmin,
    isVisitor,
    isCollaborator,

    filter,

    setFilter,

    profile,
    isMe,
    setChangeEmailVisible,
    setChangePasswordVisible,
    setChangeAvatarVisible,

    isProfileLoaded,
    profileClicked,
  } = props;

  const navigate = useNavigate();
  const location = useLocation();

  const [deleteSelfProfileDialog, setDeleteSelfProfileDialog] = useState(false);

  const [deleteOwnerProfileDialog, setDeleteOwnerProfileDialog] =
    useState(false);

  const getUserContextOptions = () => {
    const options = [
      {
        key: "change-email",
        label: t("PeopleTranslations:EmailChangeButton"),
        onClick: () => setChangeEmailVisible(true),
        disabled: false,
        icon: EmailReactSvgUrl,
      },
      {
        key: "change-password",
        label: t("PeopleTranslations:PasswordChangeButton"),
        onClick: () => setChangePasswordVisible(true),
        disabled: false,
        icon: SecurityReactSvgUrl,
      },
      {
        key: "edit-photo",
        label: t("Profile:EditPhoto"),
        onClick: () => setChangeAvatarVisible(true),
        disabled: false,
        icon: ImageReactSvgUrl,
      },
      { key: "separator", isSeparator: true },
      {
        key: "delete-profile",
        label: t("PeopleTranslations:DeleteSelfProfile"),
        onClick: () =>
          profile?.isOwner
            ? setDeleteOwnerProfileDialog(true)
            : setDeleteSelfProfileDialog(true),
        disabled: false,
        icon: CatalogTrashReactSvgUrl,
      },
    ];

    return options;
  };

  const onClickBack = () => {
    if (location?.state?.fromUrl && profileClicked) {
      return navigate(location?.state?.fromUrl);
    }

    if (location.pathname.includes("portal-settings")) {
      return navigate("/portal-settings/customization/general");
    }

    const roomsFilter = RoomsFilter.getDefault();

    roomsFilter.searchArea = RoomSearchArea.Active;
    const urlParams = roomsFilter.toUrlParams();
    const backUrl = `/rooms/shared/filter?${urlParams}`;

    navigate(backUrl);
    // setFilter(filter);
  };

  if (!isProfileLoaded) return <Loaders.SectionHeader />;

  return (
    <StyledHeader
      showContextButton={(isAdmin && !profile?.isOwner) || isMe}
      isVisitor={isVisitor || isCollaborator}
    >
      <IconButton
        iconName={ArrowPathReactSvgUrl}
        size="17"
        isFill={true}
        onClick={onClickBack}
        className="arrow-button"
      />

      <Headline className="header-headline" type="content" truncate={true}>
        {t("Profile:MyProfile")}
        {profile?.isLDAP && ` (${t("PeopleTranslations:LDAPLbl")})`}
      </Headline>
      {((isAdmin && !profile?.isOwner) || isMe) && (
        <ContextMenuButton
          className="action-button"
          directionX="right"
          title={t("Common:Actions")}
          iconName={VerticalDotsReactSvgUrl}
          size={17}
          getData={getUserContextOptions}
          isDisabled={false}
          usePortal={false}
        />
      )}

      {deleteSelfProfileDialog && (
        <DeleteSelfProfileDialog
          visible={deleteSelfProfileDialog}
          onClose={() => setDeleteSelfProfileDialog(false)}
          email={profile?.email}
        />
      )}

      {deleteOwnerProfileDialog && (
        <DeleteOwnerProfileDialog
          visible={deleteOwnerProfileDialog}
          onClose={() => setDeleteOwnerProfileDialog(false)}
        />
      )}
    </StyledHeader>
  );
};

export default inject(
  ({ auth, peopleStore, clientLoadingStore, profileActionsStore }) => {
    const { isAdmin } = auth;

    const { isVisitor, isCollaborator } = auth.userStore.user;

    const { targetUserStore, filterStore } = peopleStore;

    const { filter, setFilterParams } = filterStore;

    const { targetUser, isMe } = targetUserStore;

    const { isProfileLoaded } = clientLoadingStore;

    const { profileClicked } = profileActionsStore;

    const {
      setChangeEmailVisible,
      setChangePasswordVisible,
      setChangeAvatarVisible,
    } = targetUserStore;

    return {
      isAdmin,
      isVisitor,
      isCollaborator,
      filter,

      setFilter: setFilterParams,

      profile: targetUser,
      isMe,
      setChangeEmailVisible,
      setChangePasswordVisible,
      setChangeAvatarVisible,

      isProfileLoaded,
      profileClicked,
    };
  }
)(
  observer(withTranslation(["Profile", "Common", "PeopleTranslations"])(Header))
);
