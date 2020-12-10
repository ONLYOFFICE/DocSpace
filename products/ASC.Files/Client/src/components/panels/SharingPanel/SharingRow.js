import React from "react";
import { IconButton, Row, Text, Icons, DropDownItem } from "asc-web-components";
import { toastr, constants } from "asc-web-common";
import copy from "copy-to-clipboard";
import LinkRow from "./linkRow";
import AccessComboBox from "./AccessComboBox";
import equal from "fast-deep-equal/react";

const { ShareAccessRights } = constants;

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

  shouldComponentUpdate(nextProps) {
    if (!equal(this.props, nextProps)) {
      return true;
    }
    if (this.state.access !== this.props.item.access) {
      return true;
    }

    return true;
  }

  onCopyInternalLink = () => {
    const { selection, t } = this.props;

    const internalLink = selection.webUrl
      ? selection.webUrl
      : selection[0].webUrl;
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
    const { selection, item } = this.props;
    const { shareLink } = item.sharedTo;
    const itemName = selection.title ? selection.title : selection[0].title;
    const subject = `You have been granted access to the ${itemName} document`;
    const body = `You have been granted access to the ${itemName} document. Click the link below to open the document right now: ${shareLink}`;

    window.open(`mailto:?subject=${subject}&body=${body}`);
  };

  onShareTwitter = () =>
    window.open(
      `https://twitter.com/intent/tweet?text=${this.props.item.sharedTo.shareLink}`
    );

  onShareFacebook = () => window.open(`https://www.facebook.com`);
  //window.open(`https://www.facebook.com/dialog/feed?app_id=645528132139019&display=popup&link=${shareLink}`);

  render() {
    //console.log("SharingRow render");
    const {
      t,
      selection,
      item,
      isMyId,
      accessOptions,
      onChangeItemAccess,
      onRemoveUserClick,
      onShowEmbeddingPanel,
      onToggleLink,
      externalLinkData,
    } = this.props;

    const { isOwner } = item;
    const { label, name, displayName, shareLink, id } = item.sharedTo;

    const { access } = this.state;

    const linkVisible = selection && selection.length === 1 && shareLink;
    const {
      FullAccess,
      CustomFilter,
      Review,
      FormFilling,
      Comment,
      ReadOnly,
      DenyAccess,
    } = ShareAccessRights;

    const advancedOptions = (
      <>
        {accessOptions.includes("FullAccess") && (
          <DropDownItem
            label={t("FullAccess")}
            icon="AccessEditIcon"
            data-id={id}
            data-access={FullAccess}
            onClick={onChangeItemAccess}
          />
        )}

        {accessOptions.includes("FilterEditing") && (
          <DropDownItem
            label={t("CustomFilter")}
            icon="CustomFilterIcon"
            data-id={id}
            data-access={CustomFilter}
            onClick={onChangeItemAccess}
          />
        )}

        {accessOptions.includes("Review") && (
          <DropDownItem
            label={t("Review")}
            icon="AccessReviewIcon"
            data-id={id}
            data-access={Review}
            onClick={onChangeItemAccess}
          />
        )}

        {accessOptions.includes("FormFilling") && (
          <DropDownItem
            label={t("FormFilling")}
            icon="AccessFormIcon"
            data-id={id}
            data-access={FormFilling}
            onClick={onChangeItemAccess}
          />
        )}

        {accessOptions.includes("Comment") && (
          <DropDownItem
            label={t("Comment")}
            icon="AccessCommentIcon"
            data-id={id}
            data-access={Comment}
            onClick={onChangeItemAccess}
          />
        )}

        {accessOptions.includes("ReadOnly") && (
          <DropDownItem
            label={t("ReadOnly")}
            icon="EyeIcon"
            data-id={id}
            data-access={ReadOnly}
            onClick={onChangeItemAccess}
          />
        )}

        {accessOptions.includes("DenyAccess") && (
          <DropDownItem
            label={t("DenyAccess")}
            icon="AccessNoneIcon"
            data-id={id}
            data-access={DenyAccess}
            onClick={onChangeItemAccess}
          />
        )}
      </>
    );

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
        label: `${t("ShareVia")} Google Plus`,
        onClick: () => toastr.warning("Share via Google Plus"),
      },
      {
        key: "linkItem_4",
        label: `${t("ShareVia")} Facebook`,
        onClick: this.onShareFacebook,
      },
      {
        key: "linkItem_5",
        label: `${t("ShareVia")} Twitter`,
        onClick: this.onShareTwitter,
      },
      {
        key: "linkItem_6",
        isSeparator: true,
      },
      {
        key: "linkItem_7",
        label: t("Embedding"),
        onClick: () => onShowEmbeddingPanel(shareLink),
      },
    ];

    return (
      <>
        {linkVisible && (
          <>
            <LinkRow
              linkText="ExternalLink"
              options={externalLinkOptions}
              externalLinkData={externalLinkData}
              onToggleLink={onToggleLink}
              withToggle={true}
              {...this.props}
              advancedOptions={advancedOptions}
            />
            <LinkRow
              linkText="InternalLink"
              options={internalLinkData}
              {...this.props}
              advancedOptions={advancedOptions}
            />
          </>
        )}

        {!shareLink && (
          <Row
            className="sharing-row"
            key={`internal-link-key_${id}`}
            element={
              isOwner || id === isMyId ? (
                <Icons.AccessEditIcon
                  size="medium"
                  className="sharing_panel-owner-icon"
                />
              ) : (
                <AccessComboBox
                  access={access}
                  advancedOptions={advancedOptions}
                  directionX="left"
                />
              )
            }
            contextButtonSpacerWidth="0px"
          >
            <>
              {!shareLink && (
                <Text truncate className="sharing_panel-text">
                  {label ? label : name ? name : displayName}
                </Text>
              )}
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
                !shareLink && (
                  <IconButton
                    iconName="RemoveIcon"
                    id={id}
                    onClick={onRemoveUserClick}
                    className="sharing_panel-remove-icon"
                    color="#A3A9AE"
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
