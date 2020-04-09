import React from "react";
import {
  IconButton,
  ComboBox,
  Row,
  Text,
  Icons,
  DropDownItem,
  LinkWithDropdown,
  toastr,
} from "asc-web-components";
import copy from "copy-to-clipboard";

const SharingRow = (props) => {
  const {
    t,
    selection,
    item,
    index,
    isMe,
    accessOptions,
    onFullAccessClick,
    onReadOnlyClick,
    onReviewClick,
    onCommentClick,
    onFormFillingClick,
    onDenyAccessClick,
    onRemoveUserClick,
    onShowEmbeddingPanel,
  } = props;
  const linkVisible = selection.length === 1 && item.shareLink;

  const onCopyInternalLink = (link) => {
    const internalLink = link.split("&");
    copy(`${internalLink[0]}&${internalLink[1]}`);
    toastr.success(t("LinkCopySuccess"));
  };

  const embeddedComponentRender = (accessOptions, item) => (
    <ComboBox
      advancedOptions={advancedOptionsRender(accessOptions, item)}
      options={[]}
      selectedOption={{ key: 0 }}
      size="content"
      className="panel_combo-box"
      scaled={false}
      directionX="left"
      //isDisabled={isDisabled}
    >
      {React.createElement(Icons[item.rights.icon], {
        size: "medium",
        className: "sharing-access-combo-box-icon",
      })}
    </ComboBox>
  );

  const advancedOptionsRender = (accessOptions, item) => (
    <>
      {accessOptions.includes("FullAccess") && (
        <DropDownItem
          label="Full access"
          icon="AccessEditIcon"
          onClick={onFullAccessClick.bind(this, item)}
        />
      )}

      {accessOptions.includes("ReadOnly") && (
        <DropDownItem
          label="Read only"
          icon="EyeIcon"
          onClick={onReadOnlyClick.bind(this, item)}
        />
      )}

      {accessOptions.includes("Review") && (
        <DropDownItem
          label="Review"
          icon="AccessReviewIcon"
          onClick={onReviewClick.bind(this, item)}
        />
      )}

      {accessOptions.includes("Comment") && (
        <DropDownItem
          label="Comment"
          icon="AccessCommentIcon"
          onClick={onCommentClick.bind(this, item)}
        />
      )}

      {accessOptions.includes("FormFilling") && (
        <DropDownItem
          label="Form filling"
          icon="AccessFormIcon"
          onClick={onFormFillingClick.bind(this, item)}
        />
      )}
      {accessOptions.includes("DenyAccess") && (
        <DropDownItem
          label="Deny access"
          icon="AccessNoneIcon"
          onClick={onDenyAccessClick.bind(this, item)}
        />
      )}
    </>
  );

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
      label: "Copy internal link",
      onClick: onCopyInternalLink.bind(this, item.shareLink),
    },
  ];

  const externalLinkData = [
    {
      key: "linkItem_0",
      label: t("CopyExternalLink"),
      onClick: () => toastr.warning(t("CopyExternalLink")),
    },
    {
      key: "linkItem_1",
      isSeparator: true,
    },
    {
      key: "linkItem_2",
      label: `${t("ShareVia")} e-mail`,
      onClick: () => toastr.warning("Share via e-mail"),
    },
    {
      key: "linkItem_3",
      label: `${t("ShareVia")} Google Plus`,
      onClick: () => toastr.warning("Share via Google Plus"),
    },
    {
      key: "linkItem_4",
      label: `${t("ShareVia")} Facebook`,
      onClick: () => toastr.warning("Share via Facebook"),
    },
    {
      key: "linkItem_5",
      label: `${t("ShareVia")} Twitter`,
      onClick: () => toastr.warning("Share via Twitter"),
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
        <Row
          key={`external-link-key_${index}`}
          element={embeddedComponentRender(accessOptions, item)}
          contextButtonSpacerWidth="0px"
        >
          <>
            <LinkWithDropdown
              className="sharing_panel-link"
              color="black"
              dropdownType="alwaysDashed"
              data={externalLinkData}
            >
              {t("ExternalLink")}
            </LinkWithDropdown>
            <ComboBox
              className="sharing_panel-link-combo-box"
              options={options}
              isDisabled={false}
              selectedOption={options[0]}
              dropDownMaxHeight={200}
              noBorder={false}
              scaled={false}
              scaledOptions={true}
              size="content"
              onSelect={(option) => console.log("selected", option)}
            />
          </>
        </Row>
      )}
      {(linkVisible || !item.shareLink) && (
        <Row
          key={`internal-link-key_${index}`}
          element={
            item.rights.isOwner || item.id === isMe ? (
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
            {item.shareLink ? (
              <LinkWithDropdown
                className="sharing_panel-link"
                color="black"
                dropdownType="alwaysDashed"
                data={internalLinkData}
              >
                {t("InternalLink")}
              </LinkWithDropdown>
            ) : (
              !item.shareLink && (
                <Text truncate className="sharing_panel-text">
                  {item.label
                    ? item.label
                    : item.name
                    ? item.name
                    : item.displayName}
                </Text>
              )
            )}

            {item.rights.isOwner ? (
              <Text
                className="sharing_panel-remove-icon"
                //color="#A3A9AE"
              >
                {t("Owner")}
              </Text>
            ) : item.id === isMe ? (
              <Text
                className="sharing_panel-remove-icon"
                //color="#A3A9AE"
              >
                {t("AccessRightsFullAccess")}
              </Text>
            ) : item.shareLink ? (
              <ComboBox
                className="sharing_panel-link-combo-box"
                options={options}
                isDisabled={false}
                selectedOption={options[0]}
                dropDownMaxHeight={200}
                noBorder={false}
                scaled={false}
                scaledOptions={true}
                size="content"
                onSelect={(option) => console.log("selected", option)}
              />
            ) : (
              !item.shareLink && (
                <IconButton
                  iconName="RemoveIcon"
                  onClick={onRemoveUserClick.bind(this, item)}
                  className="sharing_panel-remove-icon"
                />
              )
            )}
          </>
        </Row>
      )}
    </>
  );
};

export default React.memo(SharingRow);
