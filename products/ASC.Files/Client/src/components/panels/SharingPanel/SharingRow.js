import React from "react";
import { IconButton, Row, Text, Icons, DropDownItem } from "asc-web-components";
import { toastr } from "asc-web-common";
import copy from "copy-to-clipboard";
import LinkRow from "./linkRow";
import AccessComboBox from "./AccessComboBox";

const SharingRow = (props) => {
  console.log("SharingRow render");
  const {
    t,
    selection,
    item,
    isMyId,
    accessOptions,
    onFullAccessClick,
    onReadOnlyClick,
    onReviewClick,
    onCommentClick,
    onFormFillingClick,
    onDenyAccessClick,
    onFilterEditingClick,
    onRemoveUserClick,
    onShowEmbeddingPanel,
    onToggleLink,
    externalLinkData,
  } = props;

  const { isOwner, access } = item;
  const { label, name, displayName, shareLink, id } = item.sharedTo;

  const linkVisible = selection && selection.length === 1 && shareLink;

  const advancedOptions = (
    <>
      {accessOptions.includes("FullAccess") && (
        <DropDownItem
          label={t("FullAccess")}
          icon="AccessEditIcon"
          data-id={id}
          onClick={onFullAccessClick}
        />
      )}

      {accessOptions.includes("FilterEditing") && (
        <DropDownItem
          label={t("CustomFilter")}
          icon="CustomFilterIcon"
          data-id={id}
          onClick={onFilterEditingClick}
        />
      )}

      {accessOptions.includes("Review") && (
        <DropDownItem
          label={t("Review")}
          icon="AccessReviewIcon"
          data-id={id}
          onClick={onReviewClick}
        />
      )}

      {accessOptions.includes("FormFilling") && (
        <DropDownItem
          label={t("FormFilling")}
          icon="AccessFormIcon"
          data-id={id}
          onClick={onFormFillingClick}
        />
      )}

      {accessOptions.includes("Comment") && (
        <DropDownItem
          label={t("Comment")}
          icon="AccessCommentIcon"
          data-id={id}
          onClick={onCommentClick}
        />
      )}

      {accessOptions.includes("ReadOnly") && (
        <DropDownItem
          label={t("ReadOnly")}
          icon="EyeIcon"
          data-id={id}
          onClick={onReadOnlyClick}
        />
      )}

      {accessOptions.includes("DenyAccess") && (
        <DropDownItem
          label={t("DenyAccess")}
          icon="AccessNoneIcon"
          data-id={id}
          onClick={onDenyAccessClick}
        />
      )}
    </>
  );

  const onCopyInternalLink = () => {
    const internalLink = selection.webUrl
      ? selection.webUrl
      : selection[0].webUrl;
    copy(internalLink);
    toastr.success(t("LinkCopySuccess"));
  };

  const onCopyClick = () => {
    toastr.success(t("LinkCopySuccess"));
    copy(shareLink);
  };

  const onShareEmail = () => {
    const itemName = selection.title ? selection.title : selection[0].title;
    const subject = `You have been granted access to the ${itemName} document`;
    const body = `You have been granted access to the ${itemName} document. Click the link below to open the document right now: 111${shareLink}111`;

    window.open(`mailto:?subject=${subject}&body=${body}`);
  };

  const onShareTwitter = () =>
    window.open(`https://twitter.com/intent/tweet?text=${shareLink}`);

  const onShareFacebook = () => window.open(`https://www.facebook.com`);
  /*window.open(`https://www.facebook.com/dialog/feed?app_id=645528132139019&display=popup&link=${shareLink}`);*/

  const internalLinkData = [
    {
      key: "linkItem",
      label: t("CopyInternalLink"),
      onClick: onCopyInternalLink,
    },
  ];

  const externalLinkOptions = [
    {
      key: "linkItem_0",
      label: t("CopyExternalLink"),
      onClick: onCopyClick,
    },
    {
      key: "linkItem_1",
      isSeparator: true,
    },
    {
      key: "linkItem_2",
      label: `${t("ShareVia")} e-mail`,
      onClick: onShareEmail,
    },
    {
      key: "linkItem_3",
      label: `${t("ShareVia")} Google Plus`,
      onClick: () => toastr.warning("Share via Google Plus"),
    },
    {
      key: "linkItem_4",
      label: `${t("ShareVia")} Facebook`,
      onClick: onShareFacebook,
    },
    {
      key: "linkItem_5",
      label: `${t("ShareVia")} Twitter`,
      onClick: onShareTwitter,
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
            {...props}
            advancedOptions={advancedOptions}
          />
          <LinkRow
            linkText="InternalLink"
            options={internalLinkData}
            {...props}
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
};

export default SharingRow;
