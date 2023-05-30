import React from "react";
import copy from "copy-to-clipboard";

import { objectToGetParams } from "@docspace/common/utils";
import { ShareAccessRights } from "@docspace/common/constants";

import toastr from "@docspace/components/toast/toastr";
import ToggleButton from "@docspace/components/toggle-button";
import InputBlock from "@docspace/components/input-block";
import Button from "@docspace/components/button";

import AccessComboBox from "./AccessComboBox";

import ShareIcon from "PUBLIC_DIR/images/share.react.svg";
import CodeIcon from "PUBLIC_DIR/images/code.react.svg";

import { StyledExternalLink } from "./StyledSharingPanel";
import Text from "@docspace/components/text";
import DropDownContainer from "@docspace/components/drop-down";
import DropDownItem from "@docspace/components/drop-down-item";

const ExternalLink = ({
  t,
  selection,
  externalItem,
  externalAccessOptions,
  onToggleLink,
  onShowEmbeddingPanel,
  isOpen,
  onChangeItemAccess,
  style,
  isPersonal,
}) => {
  const [shareLink, setShareLink] = React.useState("");
  const [shareActionOpen, setShareActionOpen] = React.useState(false);

  const ref = React.useRef(null);

  React.useEffect(() => {
    setShareLink(externalItem.sharedTo.shareLink);
  }, [externalItem]);

  const onToggleShareAction = React.useCallback(() => {
    setShareActionOpen((val) => !val);
  }, [ref.current]);

  const closeShareAction = React.useCallback(
    (e) => {
      if (ref.current.contains(e.target)) return;
      setShareActionOpen((val) => !val);
    },
    [ref.current]
  );

  const onCopyLinkAction = React.useCallback(() => {
    toastr.success(t("Translations:LinkCopySuccess"));
    copy(shareLink);
  }, [shareLink]);

  const onShowEmbeddingPanelAction = React.useCallback(() => {
    onShowEmbeddingPanel && onShowEmbeddingPanel();
  }, [onShowEmbeddingPanel]);

  const onShareEmail = React.useCallback(() => {
    const itemName = selection.title ? selection.title : selection[0].title;
    const subject = t("ShareEmailSubject", { itemName });
    const body = t("ShareEmailBody", { itemName, shareLink });

    const mailtoLink =
      "mailto:" +
      objectToGetParams({
        subject,
        body,
      });
    window.open(mailtoLink, "_self");

    onToggleShareAction();
  }, [onToggleShareAction, selection, t, shareLink]);

  // const onShareFacebook = () => {
  //   const facebookLink =
  //     "https://www.facebook.com/sharer/sharer.php" +
  //     objectToGetParams({
  //       u: shareLink,
  //       t: selection.title ? selection.title : selection[0].title,
  //     });

  //   window.open(facebookLink);
  // };

  const onShareTwitter = React.useCallback(() => {
    const twitterLink =
      "https://twitter.com/intent/tweet" +
      objectToGetParams({
        text: shareLink,
      });

    window.open(twitterLink, "", "width=1000,height=670");
    onToggleShareAction();
  }, [onToggleShareAction, shareLink]);

  const options = [
    {
      key: "linkItem_0",
      label: `${t("ShareVia")} e-mail`,
      onClick: onShareEmail,
    },
    // {
    //   key: "linkItem_1",
    //   label: `${t("ShareVia")} Facebook`,
    //   onClick: this.onShareFacebook,
    // },
    {
      key: "linkItem_2",
      label: `${t("ShareVia")} Twitter`,
      onClick: onShareTwitter,
    },
  ];

  return (
    <StyledExternalLink isPersonal={isPersonal} isOpen={isOpen} style={style}>
      <div className="external-link__base-line">
        <Text className="external-link__text" noSelect={true} truncate={true}>
          {t("ExternalLink")}
        </Text>
        <ToggleButton
          className="external-link__toggler"
          isChecked={isOpen}
          onChange={onToggleLink}
        />
      </div>

      {isOpen && (
        <>
          <div className="external-link__checked">
            <InputBlock
              className="external-link__input-link"
              scale={true}
              isReadOnly={true}
              placeholder={shareLink}
              isDisabled={true}
            >
              <div ref={ref} className="external-link__buttons">
                <CodeIcon
                  className="external-link__code-icon"
                  onClick={onShowEmbeddingPanelAction}
                />
                <ShareIcon
                  className="external-link__share-icon"
                  onClick={onToggleShareAction}
                />
                <DropDownContainer
                  className="external-link__share-dropdown"
                  open={shareActionOpen}
                  clickOutsideAction={closeShareAction}
                  withBackdrop={false}
                  isDefaultMode={false}
                >
                  {options.map((option) => (
                    <DropDownItem
                      key={option.key}
                      label={option.label}
                      onClick={option.onClick}
                    />
                  ))}
                </DropDownContainer>
              </div>
            </InputBlock>

            <Button
              className={"external-link__copy"}
              label={t("Common:Copy")}
              size={"small"}
              onClick={onCopyLinkAction}
            />
          </div>
          <div className="external-link__access-rights">
            <Text className="external-link__access-rights_text">
              {t("Common:AccessRights")}:
            </Text>
            <AccessComboBox
              t={t}
              access={externalItem?.access}
              directionX="right"
              directionY="bottom"
              accessOptions={externalAccessOptions}
              onAccessChange={onChangeItemAccess}
              itemId={externalItem?.sharedTo?.id}
              isDisabled={false}
              disableLink={false}
              isExternalLink={true}
              isDefaultMode={false}
              fixedDirection={true}
              isPersonal={isPersonal}
            />
          </div>
        </>
      )}
    </StyledExternalLink>
  );
};

export default React.memo(ExternalLink);
