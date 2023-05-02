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

import { StyledLinkRow } from "./StyledPublicRoom";

const LinkRow = (props) => {
  const { t, setEditLinkPanelIsVisible, ...rest } = props;

  const getData = () => {
    return [
      {
        key: "edit-link-key",
        label: t("Files:EditLink"),
        icon: SettingsReactSvgUrl,
        onClick: () => setEditLinkPanelIsVisible(true),
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
      <Avatar className="avatar" size="min" source={EyeReactSvgUrl} />

      <Link isHovered type="action" fontSize="14px" fontWeight={600}>
        {t("SharingPanel:ExternalLink")}
      </Link>

      <div className="external-row-icons">
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
  const { setEditLinkPanelIsVisible } = dialogsStore;

  return {
    setEditLinkPanelIsVisible,
  };
})(withTranslation(["SharingPanel", "Files"])(observer(LinkRow)));
