import React from "react";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import { getLogoFromPath } from "@docspace/common/utils";
import { StyledHeadline, StyledContainer } from "./StyledPublicRoom";
import ContextMenuButton from "@docspace/components/context-menu-button";

import ShareReactSvgUrl from "PUBLIC_DIR/images/share.react.svg?url";
import DownloadReactSvgUrl from "PUBLIC_DIR/images/download.react.svg?url";

const SectionHeaderContent = (props) => {
  const { t, theme, whiteLabelLogoUrls } = props;

  const logo = !theme.isBase
    ? getLogoFromPath(whiteLabelLogoUrls[0].path.dark)
    : getLogoFromPath(whiteLabelLogoUrls[0].path.light);

  const roomName = "Certifications";
  const DownloadAll = "Download all";
  const ShareRoom = "Share room";

  const onDownloadAll = () => {
    alert("onDownloadAll");
  };

  const onShareRoom = () => {
    alert("onShareRoom");
  };

  const getData = () => {
    return [
      {
        key: "public-room_edit",
        label: DownloadAll,
        icon: DownloadReactSvgUrl,
        onClick: onDownloadAll,
      },
      {
        key: "public-room_separator",
        isSeparator: true,
      },
      {
        key: "public-room_share",
        label: ShareRoom,
        icon: ShareReactSvgUrl,
        onClick: onShareRoom,
      },
    ];
  };

  return (
    <StyledContainer>
      <img className="logo-icon_svg" src={logo} />
      <div className="public-room-header_separator" />
      <StyledHeadline type="content" truncate>
        {roomName}
      </StyledHeadline>
      <ContextMenuButton
        // className="expandButton"
        getData={getData}
        // onClick={onContextMenu}
        // onClose={onHideContextMenu}
        // title={title}
      />
    </StyledContainer>
  );
};

export default inject(({ auth }) => {
  const { theme, whiteLabelLogoUrls } = auth.settingsStore;

  return {
    theme,
    whiteLabelLogoUrls,
  };
})(withTranslation("Common")(observer(SectionHeaderContent)));
