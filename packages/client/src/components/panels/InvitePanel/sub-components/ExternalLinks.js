import React, { useState, useEffect, useRef, memo, useCallback } from "react";
import { inject, observer } from "mobx-react";
import copy from "copy-to-clipboard";

import toastr from "@docspace/components/toast/toastr";
import { objectToGetParams } from "@docspace/common/utils";

import InputBlock from "@docspace/components/input-block";
import IconButton from "@docspace/components/icon-button";
import DropDown from "@docspace/components/drop-down";
import DropDownItem from "@docspace/components/drop-down-item";

import AccessSelector from "./AccessSelector";

import {
  StyledBlock,
  StyledSubHeader,
  StyledInviteInput,
  StyledInviteInputContainer,
  StyledToggleButton,
} from "../StyledInvitePanel";

const ExternalLinks = ({ t, roomId, roomType, defaultAccess, shareLinks }) => {
  const [linksVisible, setLinksVisible] = useState(false);
  const [actionLinksVisible, setActionLinksVisible] = useState(false);
  const [activeLink, setActiveLink] = useState([]);

  const inputsRef = useRef();

  const toggleLinks = (e) => {
    const link = shareLinks.find((l) => l.access === +defaultAccess);

    setActiveLink(link);
    setLinksVisible(!linksVisible);

    if (!linksVisible) copyLink(link.shareLink);
  };

  const onSelectAccess = (access) => {
    const link = shareLinks.find((l) => l.access === access.access);

    setActiveLink(link);
    copyLink(link.shareLink);
  };

  const copyLink = (link) => {
    toastr.success(t("Translations:LinkCopySuccess"));
    copy(link);
  };

  const toggleActionLinks = () => {
    setActionLinksVisible(!actionLinksVisible);
  };

  const closeActionLinks = () => {
    setActionLinksVisible(false);
  };

  const shareEmail = useCallback(
    (link) => {
      const { title, shareLink } = link;
      const subject = t("SharingPanel:ShareEmailSubject", { title });
      const body = t("SharingPanel:ShareEmailBody", { title, shareLink });

      const mailtoLink =
        "mailto:" +
        objectToGetParams({
          subject,
          body,
        });

      window.open(mailtoLink, "_self");

      closeActionLinks();
    },
    [closeActionLinks, t]
  );

  const shareTwitter = useCallback(
    (link) => {
      const { shareLink } = link;

      const twitterLink =
        "https://twitter.com/intent/tweet" +
        objectToGetParams({
          text: shareLink,
        });

      window.open(twitterLink, "", "width=1000,height=670");

      closeActionLinks();
    },
    [closeActionLinks]
  );

  return (
    <StyledBlock noPadding ref={inputsRef}>
      <StyledSubHeader inline>
        {t("SharingPanel:ExternalLink")}
        {false && ( //TODO: Change to linksVisible after added link information from backend
          <div style={{ position: "relative" }}>
            <IconButton
              size={16}
              iconName="/static/images/media.download.react.svg"
              hoverColor="#333333"
              iconColor="#A3A9AE"
              onClick={toggleActionLinks}
            />
            <DropDown
              open={actionLinksVisible}
              clickOutsideAction={closeActionLinks}
              withBackdrop={false}
              isDefaultMode={false}
              fixedDirection={true}
            >
              <DropDownItem
                label={`${t("SharingPanel:ShareVia")} e-mail`}
                onClick={() => shareEmail(links[0])}
              />
              <DropDownItem
                label={`${t("SharingPanel:ShareVia")} Twitter`}
                onClick={() => shareTwitter(links[0])}
              />
            </DropDown>
          </div>
        )}
        <StyledToggleButton isChecked={linksVisible} onChange={toggleLinks} />
      </StyledSubHeader>
      {linksVisible && (
        <StyledInviteInputContainer key={activeLink.id}>
          <StyledInviteInput>
            <InputBlock
              scale
              value={activeLink.shareLink}
              isReadOnly
              iconName="/static/images/copy.react.svg"
              onIconClick={() => copyLink(activeLink.shareLink)}
              hoverColor="#333333"
              iconColor="#A3A9AE"
            />
          </StyledInviteInput>
          {roomId === -1 && (
            <AccessSelector
              t={t}
              roomType={roomType}
              defaultAccess={activeLink.access}
              onSelectAccess={onSelectAccess}
              containerRef={inputsRef}
            />
          )}
        </StyledInviteInputContainer>
      )}
    </StyledBlock>
  );
};

export default inject(({ dialogsStore, filesStore }) => {
  const { invitePanelOptions } = dialogsStore;

  return {
    roomId: invitePanelOptions.roomId,
    hideSelector: invitePanelOptions.hideSelector,
    defaultAccess: invitePanelOptions.defaultAccess,
  };
})(observer(ExternalLinks));
