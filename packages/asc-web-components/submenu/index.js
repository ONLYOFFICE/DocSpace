import PropTypes from "prop-types";
import React, { useState } from "react";

import Text from "../text";
import {
  StyledSubmenu,
  StyledSubmenuBottomLine,
  StyledSubmenuContentWrapper,
  StyledSubmenuItem,
  StyledSubmenuItemLabel,
  StyledSubmenuItems,
  StyledSubmenuItemText,
} from "./styled-submenu";

const Submenu = ({ data, startSelect = 0 }) => {
  if (!data) return null;

  const [currentItem, setCurrentItem] = useState(
    data[startSelect] || startSelect || null
  );

  const onSelectSubmenuItem = (e) => {
    const item = data.find((el) => el.id === e.target.title);
    console.log(e.toString());
    if (item) setCurrentItem(item);
  };

  return (
    <StyledSubmenu>
      <StyledSubmenuItems>
        {data.map((d) => {
          const isActive = d === currentItem;
          return (
            <StyledSubmenuItem
              key={d.id}
              onClick={(e) => onSelectSubmenuItem(e)}
            >
              <StyledSubmenuItemText>
                <Text
                  style={{ cursor: "pointer" }}
                  title={d.id}
                  color={isActive ? "#316DAA" : "#657077"}
                  fontSize="13px"
                  fontWeight="600"
                  truncate="false"
                >
                  {d.name}
                </Text>
              </StyledSubmenuItemText>
              <StyledSubmenuItemLabel color={isActive ? "#316DAA" : "none"} />
            </StyledSubmenuItem>
          );
        })}
      </StyledSubmenuItems>
      <StyledSubmenuBottomLine />

      <StyledSubmenuContentWrapper>
        {currentItem.content}
      </StyledSubmenuContentWrapper>
    </StyledSubmenu>
  );
};

Submenu.propTypes = {
  data: PropTypes.arrayOf(PropTypes.object.isRequired).isRequired,
  startSelect: PropTypes.oneOfType([PropTypes.object, PropTypes.number]),
};

export default Submenu;
