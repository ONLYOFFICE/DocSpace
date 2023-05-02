import React, { useState } from "react";
import Text from "@docspace/components/text";
import IconButton from "@docspace/components/icon-button";
import toastr from "@docspace/components/toast/toastr";
import LinksToViewingIcon from "PUBLIC_DIR/images/links-to-viewing.react.svg?url";
import PublicRoomBar from "./PublicRoomBar";
import { LinksBlock } from "./StyledPublicRoom";
import LinkRow from "./LinkRow";

const PublicRoomBlock = ({ t, onCopyLink }) => {
  const [barIsVisible, setBarVisible] = useState(true);

  const onClose = () => {
    setBarVisible(!barIsVisible);
    toastr.success("onCloseBar");
  };

  return (
    <>
      {barIsVisible && (
        <PublicRoomBar
          headerText={t("CreateEditRoomDialog:PublicRoomTitle")}
          bodyText={t("CreateEditRoomDialog:PublicRoomBarDescription")}
          onClose={onClose}
        />
      )}
      <LinksBlock>
        <Text fontSize="14px" fontWeight={600} color="#a3a9ae">
          {t("LinksToViewingIcon")}
        </Text>
        <IconButton
          className="link-to-viewing-icon"
          iconName={LinksToViewingIcon}
          onClick={onCopyLink}
          size={16}
        />
      </LinksBlock>

      <LinkRow />
    </>
  );
};

export default PublicRoomBlock;
