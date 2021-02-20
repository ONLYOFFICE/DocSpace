import React from "react";
import { IconButton, Row, Text, Icons, Link } from "asc-web-components";
import { toastr } from "asc-web-common";
import copy from "copy-to-clipboard";
import LinkRow from "./linkRow";
import AccessComboBox from "./AccessComboBox";
//import equal from "fast-deep-equal/react";
import { getAccessIcon } from "../../../helpers/files-helpers";

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

  // shouldComponentUpdate(nextProps) {
  //   if (!equal(this.props, nextProps)) {
  //     return true;
  //   }
  //   if (this.state.access !== this.props.item.access) {
  //     return true;
  //   }

  //   return true;
  // }

  onCopyInternalLink = () => {
    const { internalLink, t } = this.props;

    copy(internalLink);
    toastr.success(t("LinkCopySuccess"));
  };

  onCopyClick = () => {
    const { t, item } = this.props;
    const { shareLink } = item.sharedTo;
    toastr.success(t("LinkCopySuccess"));
    copy(shareLink);
  };

  onShareEmail = () => {
    const { selection, item, t } = this.props;
    const { shareLink } = item.sharedTo;
    const itemName = selection.title ? selection.title : selection[0].title;
    const subject = t("ShareEmailSubject", { itemName });
    const body = t("ShareEmailBody", { itemName, shareLink });

    window.open(`mailto:?subject=${subject}&body=${body}`);
  };

  onShareTwitter = () => {
    const encodedLink = encodeURIComponent(this.props.item.sharedTo.shareLink);
    window.open(`https://twitter.com/intent/tweet?text=${encodedLink}`);
  };

  onShareFacebook = () => {
    window.open(
      `https://www.facebook.com/sharer/sharer.php?u=${this.props.item.sharedTo.shareLink}`
    );
  };

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
      canShareOwnerChange,
      onShowChangeOwnerPanel,
      isLoading,
      internalLink,
    } = this.props;
    const { access } = this.state;

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
        key: "linkItem_0",
        label: t("CopyExternalLink"),
        onClick: this.onCopyClick,
      },
      {
        key: "linkItem_1",
        isSeparator: true,
      },
      {
        key: "linkItem_2",
        label: `${t("ShareVia")} e-mail`,
        onClick: this.onShareEmail,
      },
      {
        key: "linkItem_3",
        label: `${t("ShareVia")} Facebook`,
        onClick: this.onShareFacebook,
      },
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

    const accessIcon = getAccessIcon(access);

    return (
      <>
        {externalLinkVisible && (
          <LinkRow
            linkText="ExternalLink"
            options={externalLinkOptions}
            externalLinkData={externalLinkData}
            onToggleLink={onToggleLink}
            withToggle
            {...this.props}
          />
        )}
        {internalLinkVisible && (
          <LinkRow
            linkText="InternalLink"
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
                React.createElement(Icons[accessIcon], {
                  size: "medium",
                  className: "sharing_panel-owner-icon",
                  isfill: true,
                  color: isLoading ? "#D0D5DA" : "#a3a9ae",
                })
              ) : (
                <AccessComboBox
                  t={t}
                  access={access}
                  directionX="left"
                  onAccessChange={onChangeItemAccess}
                  itemId={id}
                  accessOptions={accessOptions}
                  isDisabled={isLoading}
                />
              )
            }
            contextButtonSpacerWidth="0px"
          >
            <>
              {!shareLink &&
                (isOwner && canShareOwnerChange ? (
                  <Link isHovered type="action" {...onShowChangeOwnerPanelProp}>
                    {label ? label : userName ? userName : displayName}
                  </Link>
                ) : (
                  <Text truncate className="sharing_panel-text">
                    {label ? label : userName ? userName : displayName}
                  </Text>
                ))}
              {isOwner ? (
                <Text className="sharing_panel-remove-icon" color="#A3A9AE">
                  {t("Owner")}
                </Text>
              ) : id === isMyId ? (
                <Text
                  className="sharing_panel-remove-icon"
                  //color="#A3A9AE"
                >
                  {t("AccessRightsFullAccess")}
                </Text>
              ) : (
                !shareLink &&
                !isLocked && (
                  <IconButton
                    iconName="RemoveIcon"
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
    );
  }
}

export default SharingRow;
