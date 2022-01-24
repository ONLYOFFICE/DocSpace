import React from "react";
import Badge from "../../badge";
import PropTypes from "prop-types";

import {
  StyledAccessRightDescriptionItem,
  StyledAccessRightItem,
} from "./styled-accessrightitem";

const AccessRightItem = ({ title, description, icon, quota, ...props }) => {
  return (
    <StyledAccessRightItem>
      <img src={icon} style={{ marginRight: "8px" }} />
      <div style={{ width: "100%", whiteSpace: "normal" }}>
        <div style={{ display: "flex", alignItems: "center" }}>
          {title}
          {quota && (
            <Badge
              label={quota.quota}
              backgroundColor={quota.color}
              fontSize="8px"
            />
          )}
        </div>
        <StyledAccessRightDescriptionItem>
          {description}
        </StyledAccessRightDescriptionItem>
      </div>
    </StyledAccessRightItem>
  );
};

AccessRightItem.propTypes = {
  title: PropTypes.string,
  description: PropTypes.string,
  icon: PropTypes.node,
  quota: PropTypes.arrayOf(PropTypes.object),
  /** Accepts id */
  id: PropTypes.string,
  /** Accepts class */
  className: PropTypes.string,
  /** Accepts CSS style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
};

export default AccessRightItem;
