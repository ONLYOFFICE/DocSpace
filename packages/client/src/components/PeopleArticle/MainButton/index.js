import InvitationLinkReactSvgUrl from "PUBLIC_DIR/images/invitation.link.react.svg?url";
import PlaneReactSvgUrl from "PUBLIC_DIR/images/plane.react.svg?url";
import ImportReactSvgUrl from "PUBLIC_DIR/images/import.react.svg?url";
import AddDepartmentReactSvgUrl from "PUBLIC_DIR/images/add.department.react.svg?url";
import AddGuestReactSvgUrl from "PUBLIC_DIR/images/add.guest.react.svg?url";
import AddEmployeeReactSvgUrl from "ASSETS/images/add.employee.react.svg?url";
import React from "react";
//import PropTypes from "prop-types";
import { useNavigate } from "react-router-dom";
import MainButton from "@docspace/components/main-button";
import InviteDialog from "../../dialogs/InviteDialog/index";
import { withTranslation } from "react-i18next";
import toastr from "@docspace/components/toast/toastr";
import Loaders from "@docspace/common/components/Loaders";
import { inject, observer } from "mobx-react";
import config from "PACKAGE_FILE";
import { combineUrl } from "@docspace/common/utils";
import { isMobile } from "react-device-detect";
import {
  isMobile as isMobileUtils,
  isTablet as isTabletUtils,
} from "@docspace/components/utils/device";
import MobileView from "./MobileView";

import withLoader from "../../../HOCs/withLoader";

const ArticleMainButtonContent = (props) => {
  const [dialogVisible, setDialogVisible] = React.useState(false);
  const {
    homepage,
    toggleShowText,
    t,
    isAdmin,

    userCaption,

    sectionWidth,

    isMobileArticle,
  } = props;

  const navigate = useNavigate();

  const goToEmployeeCreate = () => {
    navigate(
      combineUrl(window.DocSpaceConfig?.proxy?.url, homepage, "/create/user")
    );
    if (isMobile || isMobileUtils()) toggleShowText();
  };

  const onInvitationDialogClick = () => setDialogVisible((val) => !val);

  const separator = {
    key: "separator",
    isSeparator: true,
  };

  const menuModel = [
    {
      key: "create-user",
      icon: combineUrl(
        window.DocSpaceConfig?.proxy?.url,
        homepage,
        AddEmployeeReactSvgUrl
      ),
      label: userCaption,
      onClick: goToEmployeeCreate,
    },
  ];

  const links = [
    {
      key: "invite-link",
      icon: combineUrl(
        window.DocSpaceConfig?.proxy?.url,
        InvitationLinkReactSvgUrl
      ),
      label: t("PeopleTranslations:InviteLinkTitle"),
      onClick: onInvitationDialogClick,
    },
  ];

  return isAdmin ? (
    <>
      {isMobileArticle ? (
        <MobileView
          labelProps={t("Common:OtherOperations")}
          actionOptions={menuModel}
          buttonOptions={links}
          sectionWidth={sectionWidth}
        />
      ) : (
        <MainButton
          isDisabled={false}
          isDropdown={true}
          text={t("Common:Actions")}
          model={[...menuModel, separator, ...links]}
          className="main-button_invitation-link"
        />
      )}

      {dialogVisible && (
        <InviteDialog
          visible={dialogVisible}
          onClose={onInvitationDialogClick}
          onCloseButton={onInvitationDialogClick}
        />
      )}
    </>
  ) : (
    <></>
  );
};

export default inject(({ auth }) => {
  const { userCaption, guestCaption, groupCaption } =
    auth.settingsStore.customNames;

  return {
    isAdmin: auth.isAdmin,
    homepage: config.homepage,
    userCaption,
    guestCaption,
    groupCaption,
    toggleShowText: auth.settingsStore.toggleShowText,
    isMobileArticle: auth.settingsStore.isMobileArticle,
    showText: auth.settingsStore.showText,
  };
})(
  withTranslation(["Article", "Common", "PeopleTranslations"])(
    withLoader(observer(ArticleMainButtonContent))(<Loaders.ArticleButton />)
  )
);
