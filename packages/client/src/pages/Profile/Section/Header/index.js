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
        icon: "/static/images/email.react.svg",
      },
      {
        key: "change-password",
        label: t("PeopleTranslations:PasswordChangeButton"),
        onClick: () => setChangePasswordVisible(true),
        disabled: false,
        icon: "/static/images/security.react.svg",
      },
      {
        key: "edit-photo",
        label: t("Profile:EditPhoto"),
        onClick: () => setChangeAvatarVisible(true),
        disabled: false,
        icon: "/static/images/image.react.svg",
      },
      { key: "separator", isSeparator: true },
      {
        key: "delete-profile",
        label: t("PeopleTranslations:DeleteSelfProfile"),
        onClick: () => setDeleteSelfProfileDialog(true),
        disabled: false,
        icon: "/static/images/catalog.trash.react.svg",
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
      isVisitor={!isAdmin}
    >
      {isAdmin && (
        <IconButton
          iconName="/static/images/arrow.path.react.svg"
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
          iconName="/static/images/vertical-dots.react.svg"
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
