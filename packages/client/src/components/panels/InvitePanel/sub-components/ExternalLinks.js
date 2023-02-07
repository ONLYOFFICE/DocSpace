import MediaDownloadReactSvgUrl from "PUBLIC_DIR/images/media.download.react.svg?url";
import CopyReactSvgUrl from "PUBLIC_DIR/images/copy.react.svg?url";
import React, { useState, useEffect, useRef, useCallback } from "react";
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

const ExternalLinks = ({
  t,
  roomId,
  roomType,
  defaultAccess,
  shareLinks,
  setInvitationLinks,
  isOwner,
  getInfo,
}) => {
  const [linksVisible, setLinksVisible] = useState(false);
  const [actionLinksVisible, setActionLinksVisible] = useState(false);
  const [activeLink, setActiveLink] = useState({});

  const inputsRef = useRef();

  useEffect(() => {
    if (shareLinks[0]?.expirationDate) toggleLinks();
  }, [shareLinks]);

  const toggleLinks = (e) => {
    let link = null;
    if (!shareLinks.length) return;

    if (roomId === -1) {
      link = shareLinks.find((l) => l.access === +defaultAccess);

      setActiveLink(link);
    } else {
      link = shareLinks[0];

      !linksVisible ? editLink() : disableLink();
    }

    setLinksVisible(!linksVisible);

    if (!linksVisible) copyLink(link?.shareLink);
  };

  const disableLink = () => {
    setInvitationLinks(roomId, shareLinks[0].id, "Invite", 0);
    setTimeout(() => getInfo(), 100);
  };

  const editLink = () => {
    if (!shareLinks[0].expirationDate) {
      setInvitationLinks(
        roomId,
        shareLinks[0].id,
        "Invite",
        shareLinks[0].access
      );
    }
    setActiveLink(shareLinks[0]);
  };

  const onSelectAccess = (access) => {
    let link = null;
    if (roomId === -1) {
      link = shareLinks.find((l) => l.access === access.access);

      setActiveLink(link);
    } else {
      setInvitationLinks(roomId, shareLinks[0].id, "Invite", +access.access);

      link = shareLinks[0];
      setActiveLink(shareLinks[0]);
    }

    copyLink(link.shareLink);
  };

  const copyLink = (link) => {
    if (link) {
      toastr.success(
        `${t("Translations:LinkCopySuccess")}. ${t(
          "Translations:LinkValidTime",
          { days_count: 7 }
        )}`
      );
      copy(link);
    }
  };

  const onCopyLink = () => copyLink(activeLink.shareLink);

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
              iconName={MediaDownloadReactSvgUrl}
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
                onClick={() => shareEmail(activeLink[0])}
              />
              <DropDownItem
                label={`${t("SharingPanel:ShareVia")} Twitter`}
                onClick={() => shareTwitter(activeLink[0])}
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
              className="input-link"
              scale
              value={activeLink.shareLink}
              isReadOnly
              iconName={CopyReactSvgUrl}
              onIconClick={onCopyLink}
              hoverColor="#333333"
              iconColor="#A3A9AE"
            />
          </StyledInviteInput>
          <AccessSelector
            t={t}
            roomType={roomType}
            defaultAccess={activeLink.access}
            onSelectAccess={onSelectAccess}
            containerRef={inputsRef}
            isOwner={isOwner}
          />
        </StyledInviteInputContainer>
      )}
    </StyledBlock>
  );
};

export default inject(({ auth, dialogsStore, filesStore }) => {
  const { isOwner } = auth.userStore.user;
  const { invitePanelOptions } = dialogsStore;
  const { setInvitationLinks } = filesStore;
  const { roomId, hideSelector, defaultAccess } = invitePanelOptions;

  return {
    setInvitationLinks,
    roomId,
    hideSelector,
    defaultAccess,
    isOwner,
  };
})(observer(ExternalLinks));
