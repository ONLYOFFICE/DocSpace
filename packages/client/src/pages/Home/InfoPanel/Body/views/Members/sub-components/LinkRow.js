import React from "react";
import { observer, inject } from "mobx-react";
import { withTranslation } from "react-i18next";
import Avatar from "@docspace/components/avatar";
import Link from "@docspace/components/link";
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
import ClockReactSvg from "PUBLIC_DIR/images/clock.react.svg";

import { StyledLinkRow } from "./StyledPublicRoom";

const LinkRow = (props) => {
  const { t, setEditLinkPanelIsVisible, setLinkIsEdit, link, ...rest } = props;
  const { expiryDate } = link;

  const date = "October 5, 2023 11:00 AM.";
  const tooltipContent = `This link is valid until ${date} Once it expires, it will be impossible to access the room via this link.`;

  const onEditLink = () => {
    setEditLinkPanelIsVisible(true);
    setLinkIsEdit(true);
  };

  const onLockClick = () => {
    alert("onLockClick");
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
      {
        key: "disable-link-key",
        label: t("Files:DisableLink"),
        icon: OutlineReactSvgUrl,
        // onClick: () => args.onClickLabel("label2"),
      },
    ];
  };

  const onCopyExternalLink = () => {
    toastr.success("onCopyExternalLink");
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
      >
        {link.label}
      </Link>

      <div className="external-row-icons">
        {link.locked && (
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

export default inject(({ dialogsStore }) => {
  const { setEditLinkPanelIsVisible, setLinkIsEdit } = dialogsStore;

  return {
    setEditLinkPanelIsVisible,
    setLinkIsEdit,
  };
})(withTranslation(["SharingPanel", "Files"])(observer(LinkRow)));
