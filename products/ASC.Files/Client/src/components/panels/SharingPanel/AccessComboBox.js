import React from "react";
import { ComboBox, Icons, DropDownItem } from "asc-web-components";
import { constants } from "asc-web-common";
import { getAccessIcon } from "../../../helpers/files-helpers";

const { ShareAccessRights } = constants;

const AccessComboBox = (props) => {
  const {
    access,
    accessOptions,
    directionX,
    isDisabled,
    itemId,
    onAccessChange,
    t,
    arrowIconColor,
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

  const advancedOptions = (
    <>
      {accessOptions.includes("FullAccess") && (
        <DropDownItem
          label={t("FullAccess")}
          icon="AccessEditIcon"
          data-id={itemId}
          data-access={FullAccess}
          onClick={onAccessChange}
        />
      )}

      {accessOptions.includes("FilterEditing") && (
        <DropDownItem
          label={t("CustomFilter")}
          icon="CustomFilterIcon"
          data-id={itemId}
          data-access={CustomFilter}
          onClick={onAccessChange}
        />
      )}

      {accessOptions.includes("Review") && (
        <DropDownItem
          label={t("Review")}
          icon="AccessReviewIcon"
          data-id={itemId}
          data-access={Review}
          onClick={onAccessChange}
        />
      )}

      {accessOptions.includes("FormFilling") && (
        <DropDownItem
          label={t("FormFilling")}
          icon="AccessFormIcon"
          data-id={itemId}
          data-access={FormFilling}
          onClick={onAccessChange}
        />
      )}

      {accessOptions.includes("Comment") && (
        <DropDownItem
          label={t("Comment")}
          icon="AccessCommentIcon"
          data-id={itemId}
          data-access={Comment}
          onClick={onAccessChange}
        />
      )}

      {accessOptions.includes("ReadOnly") && (
        <DropDownItem
          label={t("ReadOnly")}
          icon="EyeIcon"
          data-id={itemId}
          data-access={ReadOnly}
          onClick={onAccessChange}
        />
      )}

      {accessOptions.includes("DenyAccess") && (
        <DropDownItem
          label={t("DenyAccess")}
          icon="AccessNoneIcon"
          data-id={itemId}
          data-access={DenyAccess}
          onClick={onAccessChange}
        />
      )}
    </>
  );

  const accessIcon = getAccessIcon(access);
  const selectedOption = arrowIconColor
    ? { key: 0, arrowIconColor }
    : { key: 0 };

  return (
    <ComboBox
      advancedOptions={advancedOptions}
      options={[]}
      selectedOption={selectedOption}
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
