import PropTypes from "prop-types";
import React, { useEffect, useRef, useState } from "react";

import Text from "../text";
import { tablet } from "../utils/device";
import DomHelpers from "../utils/domHelpers";
import { countAutoOffset } from "./autoOffset";
import {
  StyledSubmenu,
  StyledSubmenuBottomLine,
  StyledSubmenuContentWrapper,
  StyledSubmenuItem,
  StyledSubmenuItemLabel,
  StyledSubmenuItems,
  StyledSubmenuItemText,
} from "./styled-submenu";

const Submenu = ({ data, startSelect = 0, ...rest }) => {
  if (!data) return null;

  const [currentItem, setCurrentItem] = useState(
    data[startSelect] || startSelect || null
  );

  const selectSubmenuItem = (e) => {
    const item = data.find((el) => el.id === e.currentTarget.id);
    if (item) setCurrentItem(item);
  };

  const submenuItemsRef = useRef();

  if (submenuItemsRef.current) {
    const offset = countAutoOffset(data, submenuItemsRef);
    submenuItemsRef.current.scrollLeft += offset;
  }

  useEffect(() => {
    if (!submenuItemsRef.current) return;
    let isDown = false;
    let startX;
    let scrollLeft;

    submenuItemsRef.current.addEventListener("mousedown", (e) => {
      e.preventDefault();
      isDown = true;
      startX = e.pageX - submenuItemsRef.current.offsetLeft;
      scrollLeft = submenuItemsRef.current.scrollLeft;
    });

    submenuItemsRef.current.addEventListener("mousemove", (e) => {
      if (!isDown) return;
      e.preventDefault();
      const x = e.pageX - submenuItemsRef.current.offsetLeft;
      const walk = x - startX;
      submenuItemsRef.current.scrollLeft = scrollLeft - walk;
    });

    submenuItemsRef.current.addEventListener("mouseup", () => {
      const offset = countAutoOffset(data, submenuItemsRef);
      submenuItemsRef.current.scrollLeft += offset;
      isDown = false;
    });
    submenuItemsRef.current.addEventListener("mouseleave", () => {
      isDown = false;
    });
  }, [submenuItemsRef]);

  return (
    <StyledSubmenu {...rest}>
      <StyledSubmenuItems ref={submenuItemsRef} role="list">
        {data.map((d) => {
          const isActive = d === currentItem;
          return (
            <StyledSubmenuItem
              key={d.id}
              _key={d.id}
              onClick={selectSubmenuItem}
            >
              <StyledSubmenuItemText>
                <Text
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
