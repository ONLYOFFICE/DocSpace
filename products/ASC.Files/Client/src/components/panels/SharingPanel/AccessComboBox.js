import React from "react";
import { ComboBox, Icons } from "asc-web-components";

const AccessComboBox = (props) => {
  const { access, advancedOptions, directionX } = props;

  const getAccessIcon = () => {
    switch (access) {
      case 1:
        return "AccessEditIcon";
      case 2:
        return "EyeIcon";
      case 3:
        return "AccessNoneIcon";
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
    >
      {React.createElement(Icons[accessIcon], {
        size: "medium",
        className: "sharing-access-combo-box-icon",
      })}
    </ComboBox>
  );
};

export default AccessComboBox;
