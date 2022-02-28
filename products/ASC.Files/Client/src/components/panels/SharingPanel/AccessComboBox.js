import React from "react";
import ComboBox from "@appserver/components/combobox";
import { ShareAccessRights } from "@appserver/common/constants";
import DropDownItem from "@appserver/components/drop-down-item";
import { getAccessIcon } from "../../../helpers/files-helpers";
import { ReactSVG } from "react-svg";

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
    disableLink,
    fixedDirection,
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
          label={t("Common:FullAccess")}
          icon="/static/images/access.edit.react.svg"
          data-id={itemId}
          data-access={FullAccess}
          onClick={onAccessChange}
        />
      )}

      {accessOptions.includes("FilterEditing") && (
        <DropDownItem
          label={t("CustomFilter")}
          icon="/static/images/custom.filter.react.svg"
          data-id={itemId}
          data-access={CustomFilter}
          onClick={onAccessChange}
        />
      )}

      {accessOptions.includes("Review") && (
        <DropDownItem
          label={t("Common:Review")}
          icon="/static/images/access.review.react.svg"
          data-id={itemId}
          data-access={Review}
          onClick={onAccessChange}
        />
      )}

      {accessOptions.includes("FormFilling") && (
        <DropDownItem
          label={t("FormFilling")}
          icon="/static/images/access.form.react.svg"
          data-id={itemId}
          data-access={FormFilling}
          onClick={onAccessChange}
        />
      )}

      {accessOptions.includes("Comment") && (
        <DropDownItem
          label={t("Comment")}
          icon="/static/images/access.comment.react.svg"
          data-id={itemId}
          data-access={Comment}
          onClick={onAccessChange}
        />
      )}

      {accessOptions.includes("ReadOnly") && (
        <DropDownItem
          label={t("ReadOnly")}
          icon="/static/images/eye.react.svg"
          data-id={itemId}
          data-access={ReadOnly}
          onClick={onAccessChange}
        />
      )}

      {accessOptions.includes("DenyAccess") && (
        <DropDownItem
          label={t("DenyAccess")}
          icon="/static/images/access.none.react.svg"
          data-id={itemId}
          data-access={DenyAccess}
          onClick={onAccessChange}
        />
      )}
    </>
  );

  const accessRights = disableLink ? ReadOnly : access;
  const accessIconUrl = getAccessIcon(accessRights);
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
      isDefaultMode={false}
      fixedDirection={fixedDirection}
    >
      <ReactSVG src={accessIconUrl} className="sharing-access-combo-box-icon" />
    </ComboBox>
  );
};

export default AccessComboBox;
