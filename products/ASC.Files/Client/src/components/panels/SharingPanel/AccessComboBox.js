import React from "react";
import { ComboBox, Icons, DropDownItem } from "asc-web-components";
import { constants } from "asc-web-common";

const { ShareAccessRights } = constants;

const AccessComboBox = (props) => {
  const {
    access,
    accessOptions,
    directionX,
    isDisabled,
    itemId,
    onAccessChange,
  } = props;
  const {
    FullAccess,
    CustomFilter,
    Review,
    FormFilling,
    Comment,
    ReadOnly,
    DenyAccess,
  } = ShareAccessRights;

  const getAccessIcon = () => {
    switch (access) {
      case 1:
        return "AccessEditIcon";
      case 2:
        return "EyeIcon";
      case 3:
        return "AccessNoneIcon";
      case 4:
        return "CatalogQuestionIcon";
      case 5:
        return "AccessReviewIcon";
      case 6:
        return "AccessCommentIcon";
      case 7:
        return "AccessFormIcon";
      case 8:
        return "CustomFilterIcon";
      default:
        return;
    }
  };

  const advancedOptions = (
    <>
      {accessOptions.includes("FullAccess") && (
        <DropDownItem
          label="Full access"
          icon="AccessEditIcon"
          data-id={itemId}
          data-access={FullAccess}
          onClick={onAccessChange}
        />
      )}

      {accessOptions.includes("FilterEditing") && (
        <DropDownItem
          label="Custom filter"
          icon="CustomFilterIcon"
          data-id={itemId}
          data-access={CustomFilter}
          onClick={onAccessChange}
        />
      )}

      {accessOptions.includes("Review") && (
        <DropDownItem
          label="Review"
          icon="AccessReviewIcon"
          data-id={itemId}
          data-access={Review}
          onClick={onAccessChange}
        />
      )}

      {accessOptions.includes("FormFilling") && (
        <DropDownItem
          label="Form filling"
          icon="AccessFormIcon"
          data-id={itemId}
          data-access={FormFilling}
          onClick={onAccessChange}
        />
      )}

      {accessOptions.includes("Comment") && (
        <DropDownItem
          label="Comment"
          icon="AccessCommentIcon"
          data-id={itemId}
          data-access={Comment}
          onClick={onAccessChange}
        />
      )}

      {accessOptions.includes("ReadOnly") && (
        <DropDownItem
          label="Read only"
          icon="EyeIcon"
          data-id={itemId}
          data-access={ReadOnly}
          onClick={onAccessChange}
        />
      )}

      {accessOptions.includes("DenyAccess") && (
        <DropDownItem
          label="Deny access"
          icon="AccessNoneIcon"
          data-id={itemId}
          data-access={DenyAccess}
          onClick={onAccessChange}
        />
      )}
    </>
  );

  const accessIcon = getAccessIcon();

  return (
    <ComboBox
      advancedOptions={advancedOptions}
      options={[]}
      selectedOption={{ key: 0 }}
      size="content"
      className="panel_combo-box"
      scaled={false}
      directionX={directionX}
      disableIconClick={false}
      isDisabled={isDisabled}
    >
      {React.createElement(Icons[accessIcon], {
        size: "medium",
        className: "sharing-access-combo-box-icon",
      })}
    </ComboBox>
  );
};

export default AccessComboBox;
