import EmailReactSvgUrl from "PUBLIC_DIR/images/email.react.svg?url";
import SecurityReactSvgUrl from "PUBLIC_DIR/images/security.react.svg?url";
import ImageReactSvgUrl from "PUBLIC_DIR/images/image.react.svg?url";
import CatalogTrashReactSvgUrl from "PUBLIC_DIR/images/catalog.trash.react.svg?url";
import ArrowPathReactSvgUrl from "PUBLIC_DIR/images/arrow.path.react.svg?url";
import VerticalDotsReactSvgUrl from "PUBLIC_DIR/images/vertical-dots.react.svg?url";
import React, { useState } from "react";
import { withTranslation } from "react-i18next";
import { withRouter } from "react-router";
import { inject, observer } from "mobx-react";

import IconButton from "@docspace/components/icon-button";
import ContextMenuButton from "@docspace/components/context-menu-button";
import Headline from "@docspace/common/components/Headline";
import Loaders from "@docspace/common/components/Loaders";
import { AppServerConfig } from "@docspace/common/constants";
import { DeleteSelfProfileDialog } from "SRC_DIR/components/dialogs";
import { combineUrl } from "@docspace/common/utils";
import config from "PACKAGE_FILE";

import withPeopleLoader from "SRC_DIR/HOCs/withPeopleLoader";

import { StyledHeader } from "./StyledHeader";

const Header = (props) => {
  const {
    t,
    history,
    isAdmin,
    isVisitor,
    filter,

    setFilter,

    profile,
    isMe,
    setChangeEmailVisible,
    setChangePasswordVisible,
    setChangeAvatarVisible,
  } = props;
  const [deleteSelfProfileDialog, setDeleteSelfProfileDialog] = useState(false);

  const getUserContextOptions = () => {
    return [
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
        onClick: () => setDeleteSelfProfileDialog(true),
        disabled: false,
        icon: CatalogTrashReactSvgUrl,
      },
    ];
  };

  const onClickBack = () => {
    const url = filter.toUrlParams();
    const backUrl = combineUrl(
      AppServerConfig.proxyURL,
      config.homepage,
      `/accounts/filter?/${url}`
    );

    history.push(backUrl, url);
    setFilter(filter);
  };

  return (
    <StyledHeader
      showContextButton={(isAdmin && !profile?.isOwner) || isMe}
      isVisitor={isVisitor}
    >
      {!isVisitor && (
        <IconButton
          iconName={ArrowPathReactSvgUrl}
          size="17"
          isFill={true}
          onClick={onClickBack}
          className="arrow-button"
        />
      )}
      <Headline className="header-headline" type="content" truncate={true}>
        {t("Profile:MyProfile")}
        {profile.isLDAP && ` (${t("PeopleTranslations:LDAPLbl")})`}
      </Headline>
      {((isAdmin && !profile.isOwner) || isMe) && (
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
          email={profile.email}
        />
      )}
    </StyledHeader>
  );
};

export default withRouter(
  inject(({ auth, peopleStore }) => {
    const { isAdmin } = auth;

    const { isVisitor } = auth.userStore.user;

    const { targetUserStore, filterStore } = peopleStore;

    const { filter, setFilterParams } = filterStore;

    const { targetUser, isMe } = targetUserStore;

    const {
      setChangeEmailVisible,
      setChangePasswordVisible,
      setChangeAvatarVisible,
    } = targetUserStore;

    return {
      isAdmin,
      isVisitor,
      filter,

      setFilter: setFilterParams,

      profile: targetUser,
      isMe,
      setChangeEmailVisible,
      setChangePasswordVisible,
      setChangeAvatarVisible,
    };
  })(
    observer(
      withTranslation(["Profile", "Common", "PeopleTranslations"])(
        withPeopleLoader(Header)(<Loaders.SectionHeader />)
      )
    )
  )
);
