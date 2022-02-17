import PropTypes from "prop-types";
import React, { useEffect, useRef, useState } from "react";

import Text from "../text";
import DomHelpers from "../utils/domHelpers";
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

  const ref = useRef();

  const test = useRef();
  let testText = "";
  const textWidths = data.map((d) => {
    testText = d.name;
  });

  useEffect(() => {
    //console.log(test.current ? test.current.offsetWidth : 0);
  }, [test.current]);

  useEffect(() => {
    if (!ref.current) return;
    let isDown = false;
    let startX;
    let scrollLeft;

    ref.current.addEventListener("mousedown", (e) => {
      isDown = true;
      startX = e.pageX - ref.current.offsetLeft;
      scrollLeft = ref.current.scrollLeft;
    });

    ref.current.addEventListener("mousemove", (e) => {
      if (!isDown) return;
      e.preventDefault();
      const x = e.pageX - ref.current.offsetLeft;
      const walk = x - startX;
      ref.current.scrollLeft = scrollLeft - walk;
      //console.log(ref.current.scrollLeft);
    });

    ref.current.addEventListener("mouseup", () => (isDown = false));
    ref.current.addEventListener("mouseleave", () => (isDown = false));
  }, [ref]);

  return (
    <StyledSubmenu {...rest}>
      {/* <div className="text" ref={test}>
        {testText}
      </div> */}
      <StyledSubmenuItems ref={ref} role="list">
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
