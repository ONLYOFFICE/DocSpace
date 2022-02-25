import React from "react";
import IconButton from "@appserver/components/icon-button";
import Link from "@appserver/components/link";
import Row from "@appserver/components/row";
import Text from "@appserver/components/text";
import toastr from "@appserver/components/toast/toastr";
import copy from "copy-to-clipboard";
import LinkRow from "./linkRow";
import AccessComboBox from "./AccessComboBox";
import { getAccessIcon } from "../../../helpers/files-helpers";
import { ReactSVG } from "react-svg";
import { objectToGetParams } from "@appserver/common/utils";

class SharingRow extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      access: props.item.access,
    };
  }

  componentDidUpdate() {
    const { access } = this.props.item;
    if (this.state.access !== access) {
      this.setState({ access });
    }
  }

  onCopyInternalLink = () => {
    const { internalLink, t } = this.props;

    copy(internalLink);
    toastr.success(t("Translations:LinkCopySuccess"));
  };

  onCopyClick = () => {
    const { t, item } = this.props;
    const { shareLink } = item.sharedTo;
    toastr.success(t("Translations:LinkCopySuccess"));
    copy(shareLink);
  };

  onShareEmail = () => {
    const { selection, item, t } = this.props;
    const { shareLink } = item.sharedTo;

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
  };

  onShareTwitter = () => {
    const { item } = this.props;
    const { shareLink } = item.sharedTo;

    const twitterLink =
      "https://twitter.com/intent/tweet" +
      objectToGetParams({
        text: shareLink,
      });

    window.open(twitterLink, "", "width=1000,height=670");
  };

  // onShareFacebook = () => {
  //   const { item, selection } = this.props;
  //   const { shareLink } = item.sharedTo;

  //   const facebookLink =
  //     "https://www.facebook.com/sharer/sharer.php" +
  //     objectToGetParams({
  //       u: shareLink,
  //       t: selection.title ? selection.title : selection[0].title,
  //     });

  //   window.open(facebookLink);
  // };

  render() {
    //console.log("SharingRow render");
    const {
      t,
      selection,
      item,
      index,
      isMyId,
      accessOptions,
      onChangeItemAccess,
      onRemoveUserClick,
      onShowEmbeddingPanel,
      onToggleLink,
      externalLinkData,
      onShowChangeOwnerPanel,
      isLoading,
      internalLink,
      isPersonal,
    } = this.props;
    const { access } = this.state;

    const canShareOwnerChange = this.props.canShareOwnerChange(item);

    const { isOwner, isLocked } = item;
    const { label, displayName, name, shareLink, id } = item.sharedTo;
    const userName = name
      ? name === "Everyone"
        ? t("ShareEveryone")
        : name
      : "";

    const externalLinkVisible =
      selection && selection.length === 1 && shareLink;
    const internalLinkVisible = index === 0 && internalLink;

    const internalLinkData = [
      {
        key: "linkItem",
        label: t("CopyInternalLink"),
        onClick: this.onCopyInternalLink,
      },
    ];

    const externalLinkOptions = [
      {
        key: "linkItem_2",
        label: `${t("ShareVia")} e-mail`,
        onClick: this.onShareEmail,
      },
      // {
      //   key: "linkItem_3",
      //   label: `${t("ShareVia")} Facebook`,
      //   onClick: this.onShareFacebook,
      // },
      {
        key: "linkItem_4",
        label: `${t("ShareVia")} Twitter`,
        onClick: this.onShareTwitter,
      },
      {
        key: "linkItem_5",
        isSeparator: true,
      },
      {
        key: "linkItem_6",
        label: t("Embedding"),
        onClick: () => onShowEmbeddingPanel(shareLink),
      },
    ];

    const onRemoveUserProp = !isLoading ? { onClick: onRemoveUserClick } : {};
    const onShowChangeOwnerPanelProp = !isLoading
      ? { onClick: onShowChangeOwnerPanel }
      : {};

    const accessIconUrl = getAccessIcon(access);

    return (
      <>
        {externalLinkVisible && (
          <LinkRow
            linkText={t("ExternalLink")}
            options={externalLinkOptions}
            externalLinkData={externalLinkData}
            onToggleLink={onToggleLink}
            withToggle
            onCopyLink={this.onCopyClick}
            {...this.props}
          />
        )}
        {!isPersonal && (
          <>
            {internalLinkVisible && (
              <LinkRow
                linkText={t("InternalLink")}
                options={internalLinkData}
                {...this.props}
              />
            )}

            {!shareLink && (
              <Row
                className="sharing-row"
                key={`internal-link-key_${id}`}
                element={
                  isOwner || isLocked ? (
                    <ReactSVG
                      src={accessIconUrl}
                      className="sharing_panel-owner-icon"
                      beforeInjection={(svg) => {
                        svg
                          .querySelector("path")
                          .setAttribute(
                            "fill",
                            isLoading ? "#D0D5DA" : "#a3a9ae"
                          );
                        svg.setAttribute(
                          "style",
                          `width:16px;
                  min-width:16px;
                  height:16px;
                  min-height:16px;`
                        );
                      }}
                    />
                  ) : (
                    <AccessComboBox
                      t={t}
                      access={access}
                      directionX="left"
                      onAccessChange={onChangeItemAccess}
                      itemId={id}
                      accessOptions={accessOptions}
                      isDisabled={isLoading}
                      fixedDirection={true}
                    />
                  )
                }
                contextButtonSpacerWidth="0px"
              >
                <>
                  {!shareLink &&
                    (isOwner && canShareOwnerChange ? (
                      <Link
                        isHovered
                        type="action"
                        {...onShowChangeOwnerPanelProp}
                      >
                        {label ? label : userName ? userName : displayName}
                      </Link>
                    ) : (
                      <Text truncate className="sharing_panel-text">
                        {label ? label : userName ? userName : displayName}
                      </Text>
                    ))}
                  {isOwner ? (
                    <Text className="sharing_panel-remove-icon" color="#A3A9AE">
                      {t("Common:Owner")}
                    </Text>
                  ) : id === isMyId ? (
                    <Text
                      className="sharing_panel-remove-icon"
                      //color="#A3A9AE"
                    >
                      {t("Common:FullAccess")}
                    </Text>
                  ) : (
                    !shareLink &&
                    !isLocked && (
                      <IconButton
                        iconName="/static/images/remove.react.svg"
                        id={id}
                        {...onRemoveUserProp}
                        className="sharing_panel-remove-icon"
                        color="#A3A9AE"
                        isDisabled={isLoading}
                      />
                    )
                  )}
                </>
              </Row>
            )}
          </>
        )}
      </>
    );
  }
}

export default SharingRow;
