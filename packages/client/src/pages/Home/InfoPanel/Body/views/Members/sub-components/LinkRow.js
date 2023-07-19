import React, { useState } from "react";
import { observer, inject } from "mobx-react";
import { withTranslation } from "react-i18next";
import copy from "copy-to-clipboard";
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
import moment from "moment";

import { StyledLinkRow } from "./StyledPublicRoom";

const LinkRow = (props) => {
  const {
    t,
    link,
    roomId,
    setLinkParams,
    setExternalLink,
    editExternalLink,
    setEditLinkPanelIsVisible,
    setDeleteLinkDialogVisible,
    setEmbeddingPanelIsVisible,
    isArchiveFolder,
    ...rest
  } = props;

  const [isLoading, setIsLoading] = useState(false);

  const {
    title,
    shareLink,
    id,
    password,
    disabled,
    expirationDate,
    isExpired,
  } = link.sharedTo;

  const isLocked = !!password;
  const expiryDate = !!expirationDate;
  const date = moment(expirationDate).format("LLL");

  const tooltipContent = isExpired
    ? t("Translations:LinkHasExpiredAndHasBeenDisabled")
    : t("Translations:PublicRoomLinkValidTime", { date });

  const onEditLink = () => {
    setEditLinkPanelIsVisible(true);
    setLinkParams({ isEdit: true, link });
  };

  const onDisableLink = () => {
    if (isExpired) {
      setEditLinkPanelIsVisible(true);
      setLinkParams({ isEdit: true, link });
      return;
    }

    setIsLoading(true);

    const newLink = JSON.parse(JSON.stringify(link));
    newLink.sharedTo.disabled = !newLink.sharedTo.disabled;

    editExternalLink(roomId, newLink)
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
    copy(password);
    toastr.success(t("Files:PasswordSuccessfullyCopied"));
  };

  const onEmbeddingClick = () => {
    setLinkParams({ link });
    setEmbeddingPanelIsVisible(true);
  };

  const onDeleteLink = () => {
    setLinkParams({ link });
    setDeleteLinkDialogVisible(true);
  };

  const onCopyExternalLink = () => {
    copy(shareLink);
    toastr.success(t("Files:LinkSuccessfullyCopied"));
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
      // {
      //   key: "share-key",
      //   label: t("Files:Share"),
      //   icon: ShareReactSvgUrl,
      //   // onClick: () => args.onClickLabel("label2"),
      // },
      !isExpired && {
        key: "embedding-settings-key",
        label: t("Files:EmbeddingSettings"),
        icon: CodeReactSvgUrl,
        onClick: onEmbeddingClick,
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

      {
        key: "delete-link-separator",
        isSeparator: true,
      },
      {
        key: "delete-link-key",
        label: t("Common:Delete"),
        icon: TrashReactSvgUrl,
        onClick: onDeleteLink,
      },
    ];
  };

  return (
    <StyledLinkRow {...rest} isExpired={isExpired}>
      <Avatar
        className="avatar"
        size="min"
        source={EyeReactSvgUrl}
        roleIcon={expiryDate ? <ClockReactSvg /> : null}
        withTooltip={expiryDate}
        tooltipContent={tooltipContent}
      />
      {isArchiveFolder ? (
        <Text fontSize="14px" fontWeight={600} className="external-row-link">
          {title}
        </Text>
      ) : (
        <Link
          isHovered
          type="action"
          fontSize="14px"
          fontWeight={600}
          onClick={onEditLink}
          isDisabled={disabled}
          color={disabled ? "#A3A9AE" : ""}
          className="external-row-link"
        >
          {title}
        </Link>
      )}

      {disabled && (
        <Text color={disabled ? "#A3A9AE" : ""}>{t("Settings:Disabled")}</Text>
      )}

      <div className="external-row-icons">
        {!disabled && !isArchiveFolder && (
          <>
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
          </>
        )}

        {!isArchiveFolder && (
          <ContextMenuButton getData={getData} isDisabled={false} />
        )}
      </div>
    </StyledLinkRow>
  );
};

export default inject(
  ({ auth, dialogsStore, publicRoomStore, treeFoldersStore }) => {
    const { selectionParentRoom } = auth.infoPanelStore;
    const {
      setEditLinkPanelIsVisible,
      setDeleteLinkDialogVisible,
      setEmbeddingPanelIsVisible,
      setLinkParams,
    } = dialogsStore;
    const { editExternalLink, setExternalLink } = publicRoomStore;
    const { isArchiveFolder } = treeFoldersStore;

    return {
      setLinkParams,
      editExternalLink,
      roomId: selectionParentRoom.id,
      setExternalLink,
      setEditLinkPanelIsVisible,
      setDeleteLinkDialogVisible,
      setEmbeddingPanelIsVisible,
      isArchiveFolder,
    };
  }
)(
  withTranslation(["SharingPanel", "Files", "Settings", "Translations"])(
    observer(LinkRow)
  )
);
