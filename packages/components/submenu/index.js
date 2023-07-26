import PropTypes from "prop-types";
import React, { useEffect, useRef, useState, useCallback } from "react";
import { countAutoFocus, countAutoOffset } from "./autoOffset";
import {
  StyledSubmenu,
  StyledSubmenuBottomLine,
  StyledSubmenuContentWrapper,
  StyledSubmenuItem,
  StyledSubmenuItems,
  StyledSubmenuItemText,
  SubmenuScroller,
  SubmenuRoot,
  SubmenuScrollbarSize,
} from "./styled-submenu";

import { ColorTheme, ThemeType } from "@docspace/components/ColorTheme";

const Submenu = (props) => {
  const {
    data,
    startSelect = 0,
    forsedActiveItemId,
    onSelect,
    ...rest
  } = props;
  if (!data) return null;

  const [currentItem, setCurrentItem] = useState(
    data[startSelect] || startSelect || null
  );

  useEffect(() => {
    window.addEventListener("popstate", onCheckCurrentItem);
    onCheckCurrentItem();

    return () => {
      window.removeEventListener("popstate", onCheckCurrentItem);
    };
  }, []);

  const onCheckCurrentItem = useCallback(() => {
    const isSelect = data.find((item) =>
      window.location.pathname.endsWith(item.id)
    );

    if (isSelect) setCurrentItem(isSelect);
  }, [data, setCurrentItem]);

  const submenuItemsRef = useRef();

  const selectSubmenuItem = (e) => {
    if (!forsedActiveItemId) {
      const item = data.find((el) => el.id === e.currentTarget.id);
      if (item) setCurrentItem(item);
      const offset = countAutoFocus(item.name, data, submenuItemsRef);
      submenuItemsRef.current.scrollLeft += offset;
      onSelect && onSelect(item);
    }
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
      <div className="sticky">
        <SubmenuRoot>
          <SubmenuScrollbarSize />
          <SubmenuScroller>
            <StyledSubmenuItems ref={submenuItemsRef} role="list">
              {data.map((d) => {
                const isActive =
                  d.id === (forsedActiveItemId || currentItem.id);

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
                      <ColorTheme
                        {...props}
                        themeId={ThemeType.SubmenuText}
                        className="item-text"
                        fontSize="13px"
                        fontWeight="600"
                        truncate={false}
                        isActive={isActive}
                      >
                        {d.name}
                      </ColorTheme>
                    </StyledSubmenuItemText>

                    <ColorTheme
                      {...props}
                      themeId={ThemeType.SubmenuItemLabel}
                      isActive={isActive}
                    />
                  </StyledSubmenuItem>
                );
              })}
            </StyledSubmenuItems>
          </SubmenuScroller>
        </SubmenuRoot>
        <StyledSubmenuBottomLine className="bottom-line" />
      </div>
      <div className="sticky-indent"></div>

      <StyledSubmenuContentWrapper>
        {currentItem.content}
      </StyledSubmenuContentWrapper>
    </StyledSubmenu>
  );
};

Submenu.propTypes = {
  /** List of the elements*/
  data: PropTypes.arrayOf(PropTypes.object.isRequired).isRequired,
  /** Specifies the first item or the item's index to be displayed in the submenu. */
  startSelect: PropTypes.oneOfType([PropTypes.object, PropTypes.number]),
  /** Property that allows explicitly selecting content passed through an external operation  */
  forsedActiveItemId: PropTypes.any,
  /** Sets a callback function that is triggered when the submenu item is selected */
  onSelect: PropTypes.func,
};

export default Submenu;
