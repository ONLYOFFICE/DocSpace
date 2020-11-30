import React from "react";
import {
  IconButton,
  ComboBox,
  Row,
  Text,
  Icons,
  DropDownItem,
} from "asc-web-components";
import { toastr } from "asc-web-common";
import copy from "copy-to-clipboard";
import LinkRow from "./linkRow";

const SharingRow = (props) => {
  const {
    t,
    selection,
    item,
    index,
    isMyId,
    accessOptions,
    onFullAccessClick,
    onReadOnlyClick,
    onReviewClick,
    onCommentClick,
    onFormFillingClick,
    onDenyAccessClick,
    onRemoveUserClick,
    onShowEmbeddingPanel,
    accessRight,
  } = props;

  const linkVisible = selection && selection.length === 1 && item.shareLink;
  const onCopyInternalLink = () => {
    const internalLink = selection.webUrl
      ? selection.webUrl
      : selection[0].webUrl;
    copy(internalLink);
    toastr.success(t("LinkCopySuccess"));
  };

  const advancedOptions = (
    <>
      {accessOptions.includes("FullAccess") && (
        <DropDownItem
          label="Full access"
          icon="AccessEditIcon"
          //onClick={props.onFullAccessClick}
        />
      )}

      {accessOptions.includes("ReadOnly") && (
        <DropDownItem
          label="Read only"
          icon="EyeIcon"
          //onClick={props.onReadOnlyClick}
        />
      )}

      {accessOptions.includes("Review") && (
        <DropDownItem
          label="Review"
          icon="AccessReviewIcon"
          //onClick={props.onReviewClick}
        />
      )}

      {accessOptions.includes("Comment") && (
        <DropDownItem
          label="Comment"
          icon="AccessCommentIcon"
          //onClick={props.onCommentClick}
        />
      )}

      {accessOptions.includes("FormFilling") && (
        <DropDownItem
          label="Form filling"
          icon="AccessFormIcon"
          //onClick={props.onFormFillingClick}
        />
      )}
      {accessOptions.includes("DenyAccess") && (
        <DropDownItem
          label="Deny access"
          icon="AccessNoneIcon"
          //onClick={props.onDenyAccessClick}
        />
      )}
    </>
  );

  const advancedOptionsRender = () => (
    <>
      {accessOptions.includes("FullAccess") && (
        <DropDownItem
          label="Full access"
          icon="AccessEditIcon"
          onClick={() => onFullAccessClick(item)}
        />
      )}

      {accessOptions.includes("ReadOnly") && (
        <DropDownItem
          label="Read only"
          icon="EyeIcon"
          onClick={() => onReadOnlyClick(item)}
        />
      )}

      {accessOptions.includes("Review") && (
        <DropDownItem
          label="Review"
          icon="AccessReviewIcon"
          onClick={() => onReviewClick(item)}
        />
      )}

      {accessOptions.includes("Comment") && (
        <DropDownItem
          label="Comment"
          icon="AccessCommentIcon"
          onClick={() => onCommentClick(item)}
        />
      )}

      {accessOptions.includes("FormFilling") && (
        <DropDownItem
          label="Form filling"
          icon="AccessFormIcon"
          onClick={() => onFormFillingClick(item)}
        />
      )}
      {accessOptions.includes("DenyAccess") && (
        <DropDownItem
          label="Deny access"
          icon="AccessNoneIcon"
          onClick={() => onDenyAccessClick(item)}
        />
      )}
    </>
  );

  const accessOptionsComboBox = (accessOptions, item, isDisabled) => (
    <ComboBox
      advancedOptions={advancedOptions}
      options={[]}
      selectedOption={{ key: 0 }}
      size="content"
      className="panel_combo-box"
      scaled={false}
      directionX="left"
      disableIconClick={false}
      isDisabled={isDisabled}
    >
      {React.createElement(Icons[accessRight.icon], {
        size: "medium",
        className: "sharing-access-combo-box-icon",
      })}
    </ComboBox>
  );

  const embeddedComponentRender = (accessOptions, item, isDisabled) => (
    <ComboBox
      advancedOptions={advancedOptionsRender()}
      options={[]}
      selectedOption={{ key: 0 }}
      size="content"
      className="panel_combo-box"
      scaled={false}
      directionX="left"
      disableIconClick={false}
      isDisabled={isDisabled}
    >
      {React.createElement(Icons[item.rights.icon], {
        size: "medium",
        className: "sharing-access-combo-box-icon",
      })}
    </ComboBox>
  );

  const onCopyClick = () => {
    toastr.success(t("LinkCopySuccess"));
    copy(item.shareLink);
  };

  const onShareEmail = () => {
    const itemName = selection.title ? selection.title : selection[0].title;
    const subject = `You have been granted access to the ${itemName} document`;
    const body = `You have been granted access to the ${itemName} document. Click the link below to open the document right now: 111${item.shareLink}111`;

    window.open(`mailto:?subject=${subject}&body=${body}`);
  };

  const onShareTwitter = () =>
    window.open(`https://twitter.com/intent/tweet?text=${item.shareLink}`);

  const onShareFacebook = () => window.open(`https://www.facebook.com`);
  /*window.open(`https://www.facebook.com/dialog/feed?app_id=645528132139019&display=popup&link=${item.shareLink}`);*/

  const onChangeToggle = (e) => {
    return e.target.checked;
  };

  const options = [
    {
      key: 1,
      label: "Disabled",
      disabled: false,
      onClick: () => console.log("Disabled"),
    },
    {
      key: 2,
      label: "1 hour",
      disabled: false,
      onClick: () => console.log("1 hour"),
    },
    {
      key: 3,
      label: "1 day",
      disabled: false,
      onClick: () => console.log("1 day"),
    },
    {
      key: 4,
      label: "1 week",
      disabled: false,
      onClick: () => console.log("1 week"),
    },
    {
      key: 5,
      label: "1 month",
      disabled: false,
      onClick: () => console.log("1 month"),
    },
    {
      key: 6,
      label: "1 year",
      disabled: false,
      onClick: () => console.log("1 year"),
    },
    {
      key: 7,
      label: "Timeless",
      disabled: false,
      onClick: () => console.log("Timeless"),
    },
  ];

  const internalLinkData = [
    {
      key: "linkItem",
      label: t("CopyInternalLink"),
      onClick: onCopyInternalLink,
    },
  ];

  const externalLinkData = [
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
      onClick: () => onShowEmbeddingPanel(item.shareLink),
    },
  ];

  //console.log("SharingRow render");
  return (
    <>
      {linkVisible && (
        <>
          <LinkRow
            linkText="ExternalLink"
            data={externalLinkData}
            embeddedComponentRender={accessOptionsComboBox}
            onChangeToggle={onChangeToggle}
            withToggle={true}
            {...props}
          />
          <LinkRow
            linkText="InternalLink"
            data={internalLinkData}
            embeddedComponentRender={embeddedComponentRender}
            {...props}
          />
        </>
      )}

      {!item.shareLink && (
        <Row
          className="sharing-row"
          key={`internal-link-key_${index}`}
          element={
            item.rights.isOwner || item.id === isMyId ? (
              <Icons.AccessEditIcon
                size="medium"
                className="sharing_panel-owner-icon"
              />
            ) : (
              embeddedComponentRender(accessOptions, item)
            )
          }
          contextButtonSpacerWidth="0px"
        >
          <>
            {!item.shareLink && (
              <Text truncate className="sharing_panel-text">
                {item.label
                  ? item.label
                  : item.name
                  ? item.name
                  : item.displayName}
              </Text>
            )}
            {item.rights.isOwner ? (
              <Text className="sharing_panel-remove-icon" color="#A3A9AE">
                {t("Owner")}
              </Text>
            ) : item.id === isMyId ? (
              <Text
                className="sharing_panel-remove-icon"
                //color="#A3A9AE"
              >
                {t("AccessRightsFullAccess")}
              </Text>
            ) : (
              !item.shareLink && (
                <IconButton
                  iconName="RemoveIcon"
                  onClick={() => onRemoveUserClick(item)}
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
