import React, { useState } from "react";
import { inject, observer } from "mobx-react";
import Text from "@docspace/components/text";
import IconButton from "@docspace/components/icon-button";
import toastr from "@docspace/components/toast/toastr";
import LinksToViewingIcon from "PUBLIC_DIR/images/links-to-viewing.react.svg?url";
import PublicRoomBar from "./PublicRoomBar";
import { LinksBlock } from "./StyledPublicRoom";
import LinkRow from "./LinkRow";

const PublicRoomBlock = ({ t, externalLinks, onCopyLink }) => {
  const [barIsVisible, setBarVisible] = useState(true);

  const onClose = () => {
    setBarVisible(!barIsVisible);
    toastr.success("onCloseBar");
  };

  // const defaultLink = { id: 0, label: t("SharingPanel:ExternalLink") };

  return (
    <>
      {barIsVisible && (
        <PublicRoomBar
          headerText={t("Files:PublicRoom")}
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

      {externalLinks.length ? (
        externalLinks.map(
          (link) =>
            !link.sharedTo.isTemplate && (
              <LinkRow link={link} key={link?.sharedTo?.id} />
            )
        )
      ) : (
        <>{/* <LinkRow link={defaultLink} /> */}</>
      )}
    </>
  );
};

export default inject(({ publicRoomStore }) => {
  const { externalLinks } = publicRoomStore;

  return {
    externalLinks,
  };
})(observer(PublicRoomBlock));
