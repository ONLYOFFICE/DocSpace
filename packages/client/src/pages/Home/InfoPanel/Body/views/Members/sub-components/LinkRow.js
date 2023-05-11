import React, { useState } from "react";
import { observer, inject } from "mobx-react";
import { withTranslation } from "react-i18next";
import Avatar from "@docspace/components/avatar";
import Link from "@docspace/components/link";
import Text from "@docspace/components/text";
import IconButton from "@docspace/components/icon-button";
import ContextMenuButton from "@docspace/components/context-menu-button";
import { toastr } from "@docspace/components";
import CopyReactSvgUrl from "PUBLIC_DIR/images/copy.react.svg?url";
import EyeReactSvgUrl from "PUBLIC_DIR/images/eye.react.svg?url";
import SettingsReactSvgUrl from "PUBLIC_DIR/images/catalog.settings.react.svg?url";
import ShareReactSvgUrl from "PUBLIC_DIR/images/share.react.svg?url";
import CodeReactSvgUrl from "PUBLIC_DIR/images/code.react.svg?url";
import OutlineReactSvgUrl from "PUBLIC_DIR/images/outline-true.react.svg?url";
import LockedReactSvgUrl from "PUBLIC_DIR/images/locked.react.svg?url";
import LoadedReactSvgUrl from "PUBLIC_DIR/images/loaded.react.svg?url";
import TrashReactSvgUrl from "PUBLIC_DIR/images/trash.react.svg?url";
import ClockReactSvg from "PUBLIC_DIR/images/clock.react.svg";

import { StyledLinkRow } from "./StyledPublicRoom";

const LinkRow = (props) => {
  const {
    t,
    roomId,
    setEditLinkPanelIsVisible,
    setLinkParams,
    link,
    editExternalLink,
    setExternalLink,
    canLinkDelete,
    ...rest
  } = props;
  const { sharedTo } = link;

  const [isLoading, setIsLoading] = useState(false);

  const {
    title,
    /* isLocked, */ shareLink,
    id,
    password,
    disabled,
    expirationDate,
  } = sharedTo;

  const isLocked = !!password;
  const expiryDate = !!expirationDate;

  const date = "October 5, 2023 11:00 AM.";
  const tooltipContent = `This link is valid until ${date} Once it expires, it will be impossible to access the room via this link.`;

  const onEditLink = () => {
    setEditLinkPanelIsVisible(true);
    setLinkParams({ linkId: id, isEdit: true });
  };

  const onDisableLink = () => {
    setIsLoading(true);
    editExternalLink({ roomId, linkId: id, title, disabled: !disabled })
      .then((res) => {
        setExternalLink(id, res);

        disabled
          ? toastr.success(t("Files:LinkEnabledSuccessfully"))
          : toastr.success(t("Files:LinkDisabledSuccessfully"));
      })
      .catch((err) => toastr.error(err?.message))
      .finally(() => setIsLoading(false));
  };

  const onLockClick = () => {
    alert("onLockClick");
  };

  const onDeleteLink = () => {
    alert("show delete dialog");
    setIsLoading(true);
    editExternalLink({ roomId, linkId: id, title, access: 0 })
      .then((res) => {
        console.log("res", res);
        // setExternalLink(id, res);
      })
      .catch((err) => toastr.error(err?.message))
      .finally(() => setIsLoading(false));
  };

  const getData = () => {
    return [
      {
        key: "edit-link-key",
        label: t("Files:EditLink"),
        icon: SettingsReactSvgUrl,
        onClick: onEditLink,
      },
      {
        key: "edit-link-separator",
        isSeparator: true,
      },
      {
        key: "share-key",
        label: t("Files:Share"),
        icon: ShareReactSvgUrl,
        // onClick: () => args.onClickLabel("label2"),
      },
      {
        key: "embedding-settings-key",
        label: t("Files:EmbeddingSettings"),
        icon: CodeReactSvgUrl,
        // onClick: () => args.onClickLabel("label2"),
      },
      disabled
        ? {
            key: "enable-link-key",
            label: t("Files:EnableLink"),
            icon: LoadedReactSvgUrl,
            onClick: onDisableLink,
          }
        : {
            key: "disable-link-key",
            label: t("Files:DisableLink"),
            icon: OutlineReactSvgUrl,
            onClick: onDisableLink,
          },

      canLinkDelete && {
        key: "delete-link-separator",
        isSeparator: true,
      },
      canLinkDelete && {
        key: "delete-link-key",
        label: t("Common:Delete"),
        icon: TrashReactSvgUrl,
        onClick: onDeleteLink,
      },
    ];
  };

  const onCopyExternalLink = () => {
    toastr.success("onCopyExternalLink"); // shareLink
  };

  return (
    <StyledLinkRow {...rest}>
      <Avatar
        className="avatar"
        size="min"
        source={EyeReactSvgUrl}
        roleIcon={expiryDate ? <ClockReactSvg /> : null}
        withTooltip={expiryDate}
        tooltipContent={tooltipContent}
      />

      <Link
        isHovered
        type="action"
        fontSize="14px"
        fontWeight={600}
        onClick={onEditLink}
        isDisabled={disabled}
        color={disabled ? "#A3A9AE" : ""}
      >
        {title}
      </Link>

      {disabled && (
        <Text color={disabled ? "#A3A9AE" : ""}>{t("Settings:Disabled")}</Text>
      )}

      <div className="external-row-icons">
        {isLocked && (
          <IconButton
            className="locked-icon"
            size={16}
            iconName={LockedReactSvgUrl}
            onClick={onLockClick}
          />
        )}
        <IconButton
          className="copy-icon"
          size={16}
          iconName={CopyReactSvgUrl}
          onClick={onCopyExternalLink}
        />

        <ContextMenuButton getData={getData} isDisabled={false} />
      </div>
    </StyledLinkRow>
  );
};

export default inject(({ auth, dialogsStore, publicRoomStore }) => {
  const { selectionParentRoom } = auth.infoPanelStore;
  const { setEditLinkPanelIsVisible, setLinkParams } = dialogsStore;
  const { editExternalLink, externalLinks, setExternalLink } = publicRoomStore;

  return {
    setEditLinkPanelIsVisible,
    setLinkParams,
    editExternalLink,
    roomId: selectionParentRoom.id,
    setExternalLink,
    canLinkDelete: externalLinks.length >= 1,
  };
})(withTranslation(["SharingPanel", "Files", "Settings"])(observer(LinkRow)));
