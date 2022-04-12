import React from "react";
import toastr from "@appserver/components/toast/toastr";
import copy from "copy-to-clipboard";
import { objectToGetParams } from "@appserver/common/utils";
import ExternalLink from "./ExternalLink";
import InternalLink from "./InternalLink";
import Item from "./Item";

class SharingRow extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      access: !this.props.isInternalLinkOnly ? props.item.access : "",
    };
  }

  componentDidUpdate() {
    if (!this.props.isInternalLinkOnly) {
      const { access } = this.props.item;
      if (this.state.access !== access) {
        this.setState({ access });
      }
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
    if (this.props.isInternalLinkOnly) {
      const { t } = this.props;
      return (
        <InternalLink
          linkText={t("InternalLink")}
          copyText={t("Translations:Copy")}
          onCopyInternalLink={this.onCopyInternalLink}
          {...this.props}
        />
      );
    }
    const {
      t,
      selection,
      item,
      index,
      isMyId,

      onRemoveUserClick,
      onShowEmbeddingPanel,
      onToggleLink,
      onShowChangeOwnerPanel,
      internalLink,
      isPersonal,
    } = this.props;

    const canShareOwnerChange = this.props.canShareOwnerChange(item);

    const {
      label,
      displayName,
      name,
      shareLink,
      avatarSmall,
      avatarUrl,
      id,
    } = item.sharedTo;

    const externalLinkVisible =
      selection && selection.length === 1 && shareLink;

    const internalLinkVisible = index === 0 && internalLink;

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
    ];

    const itemName =
      id === isMyId
        ? t("Common:MeLabel")
        : !!displayName
        ? displayName
        : !!name
        ? name
        : label;

    return (
      <>
        {externalLinkVisible && (
          <ExternalLink
            linkText={t("ExternalLink")}
            shareText={t("Home:Share")}
            shareLink={shareLink}
            options={externalLinkOptions}
            onToggleLink={onToggleLink}
            onCopyLink={this.onCopyClick}
            onShowEmbeddingPanel={onShowEmbeddingPanel}
            {...this.props}
          />
        )}

        {!isPersonal && internalLinkVisible && (
          <InternalLink
            linkText={t("InternalLink")}
            copyText={t("Translations:Copy")}
            onCopyInternalLink={this.onCopyInternalLink}
            {...this.props}
          />
        )}

        {!isPersonal && !externalLinkVisible && (
          <Item
            {...this.props}
            avatarUrl={!!avatarSmall ? avatarSmall : avatarUrl}
            access={item.access}
            label={itemName}
            isOwner={item.isOwner}
            ownerText={t("Common:Owner")}
            onShowChangeOwnerPanel={onShowChangeOwnerPanel}
            changeOwnerText={t("ChangeOwnerPanel:ChangeOwner").replace(
              "()",
              ""
            )}
            canShareOwnerChange={canShareOwnerChange && !shareLink}
            onRemoveUserClick={onRemoveUserClick}
          />
        )}
      </>
    );
  }
}

export default SharingRow;
