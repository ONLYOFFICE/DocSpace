import PropTypes from "prop-types";
import React, { useEffect, useRef, useState } from "react";
import Text from "../text";
import { countAutoFocus, countAutoOffset } from "./autoOffset";
import {
  StyledSubmenu,
  StyledSubmenuBottomLine,
  StyledSubmenuContentWrapper,
  StyledSubmenuItem,
  StyledSubmenuItemLabel,
  StyledSubmenuItems,
  StyledSubmenuItemText,
} from "./styled-submenu";
import LoaderSubmenu from "./loader";

const Submenu = ({ data, startSelect = 0, onSelect, isLoading, ...rest }) => {
  if (!data) return null;

  const [currentItem, setCurrentItem] = useState(
    data[startSelect] || startSelect || null
  );

  const submenuItemsRef = useRef();

  const selectSubmenuItem = (e) => {
    const item = data.find((el) => el.id === e.currentTarget.id);
    if (item) setCurrentItem(item);
    const offset = countAutoFocus(item.name, data, submenuItemsRef);
    submenuItemsRef.current.scrollLeft += offset;
    onSelect && onSelect(item);
  };

  useEffect(() => {
    if (!submenuItemsRef.current) return;
    let isDown = false;
    let startX;
    let scrollLeft;

    const mouseDown = (e) => {
      e.preventDefault();
      isDown = true;
      startX = e.pageX - submenuItemsRef.current.offsetLeft;
      scrollLeft = submenuItemsRef.current.scrollLeft;
    };

    const mouseMove = (e) => {
      if (!isDown) return;
      e.preventDefault();
      const x = e.pageX - submenuItemsRef.current.offsetLeft;
      const walk = x - startX;
      submenuItemsRef.current.scrollLeft = scrollLeft - walk;
    };

    const mouseUp = () => {
      const offset = countAutoOffset(data, submenuItemsRef);
      submenuItemsRef.current.scrollLeft += offset;
      isDown = false;
    };

    const mouseLeave = () => (isDown = false);

    submenuItemsRef.current.addEventListener("mousedown", mouseDown);
    submenuItemsRef.current.addEventListener("mousemove", mouseMove);
    submenuItemsRef.current.addEventListener("mouseup", mouseUp);
    submenuItemsRef.current.addEventListener("mouseleave", mouseLeave);

    return () => {
      submenuItemsRef.current?.removeEventListener("mousedown", mouseDown);
      submenuItemsRef.current?.removeEventListener("mousemove", mouseMove);
      submenuItemsRef.current?.removeEventListener("mouseup", mouseUp);
      submenuItemsRef.current?.removeEventListener("mouseleave", mouseLeave);
    };
  }, [submenuItemsRef]);

  return (
    <StyledSubmenu {...rest}>
      {isLoading ? (
        <LoaderSubmenu />
      ) : (
        <>
          <div className="sticky">
            <StyledSubmenuItems ref={submenuItemsRef} role="list">
              {data.map((d) => {
                const isActive = d.id === currentItem.id;

                return (
                  <StyledSubmenuItem
                    key={d.id}
                    id={d.id}
                    onClick={(e) => {
                      d.onClick && d.onClick();
                      selectSubmenuItem(e);
                    }}
                  >
                    <StyledSubmenuItemText isActive={isActive}>
                      <Text
                        className="item-text"
                        fontSize="13px"
                        fontWeight="600"
                        truncate={false}
                      >
                        {d.name}
                      </Text>
                    </StyledSubmenuItemText>
                    <StyledSubmenuItemLabel isActive={isActive} />
                  </StyledSubmenuItem>
                );
              })}
            </StyledSubmenuItems>
            <StyledSubmenuBottomLine />
          </div>
          <div className="sticky-indent"></div>
        </>
      )}
      <StyledSubmenuContentWrapper>
        {currentItem.content}
      </StyledSubmenuContentWrapper>
    </StyledSubmenu>
  );
};

Submenu.propTypes = {
  data: PropTypes.arrayOf(PropTypes.object.isRequired).isRequired,
  startSelect: PropTypes.oneOfType([PropTypes.object, PropTypes.number]),
  onSelect: PropTypes.func,
  isLoading: PropTypes.bool,
};

export default Submenu;
