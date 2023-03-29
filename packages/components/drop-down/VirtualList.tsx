import React, {
  NamedExoticComponent,
  ReactElement,
  useCallback,
  useEffect,
  useMemo,
  useRef,
  useState,
} from "react";
import { VariableSizeList } from "react-window";
import Scrollbar from "@docspace/components/scrollbar";
import CustomScrollbarsVirtualList from "../scrollbar/custom-scrollbars-virtual-list";

type VirtualListProps = {
  width: number;
  theme: unknown;
  isOpen: boolean;
  itemCount: number;
  maxHeight: number;
  calculatedHeight: number;
  isNoFixedHeightOptions: boolean;
  cleanChildren: ReactElement[];
  children: ReactElement[];
  Row: NamedExoticComponent<object>;
  enableKeyboardEvents: boolean;

  getItemSize: (index: number) => number;
};

function VirtualList({
  Row,
  width,
  theme,
  isOpen,
  children,
  itemCount,
  maxHeight,
  cleanChildren,
  calculatedHeight,
  isNoFixedHeightOptions,
  getItemSize,
  enableKeyboardEvents,
}: VirtualListProps) {
  const ref = useRef<VariableSizeList>(null);

  const activeIndex = useMemo(() => {
    let foundIndex = -1;
    React.Children.forEach(cleanChildren, (child, index) => {
      if (child.props.disabled) foundIndex = index;
    });
    return foundIndex;
  }, [cleanChildren]);

  const [currentIndex, setCurrentIndex] = useState(activeIndex);
  const currentIndexRef = useRef<number>(activeIndex);

  useEffect(() => {
    if (isOpen && maxHeight && enableKeyboardEvents) {
      window.addEventListener("keydown", onKeyDown);
    }

    return () => {
      window.removeEventListener("keydown", onKeyDown);

      if (itemCount > 0 && ref.current) {
        setCurrentIndex(activeIndex);
        currentIndexRef.current = activeIndex;

        ref.current.scrollToItem(activeIndex, "smart");
      }
    };
  }, [isOpen, activeIndex, maxHeight, enableKeyboardEvents]);

  const onKeyDown = useCallback(
    (event: KeyboardEvent) => {
      if (!ref.current || !isOpen) return;

      event.preventDefault();

      let index = currentIndexRef.current;

      switch (event.code) {
        case "ArrowDown":
          index++;
          break;
        case "ArrowUp":
          index--;
          break;
        case "Enter":
          children[index]?.props?.onClick();
          break;
        default:
          return;
      }

      if (index < 0 || index >= React.Children.count(children)) return;

      setCurrentIndex(index);
      currentIndexRef.current = index;
      ref.current.scrollToItem(index, "smart");
    },
    [isOpen]
  );

  const handleMouseMove = useCallback((index: number) => {
    if (currentIndexRef.current === index) return;

    setCurrentIndex(index);
    currentIndexRef.current = index;
  }, []);

  if (!maxHeight) return cleanChildren ? cleanChildren : children;

  return (
    <>
      {isNoFixedHeightOptions ? (
        //@ts-ignore
        <Scrollbar style={{ height: maxHeight }} stype="mediumBlack">
          {cleanChildren}
        </Scrollbar>
      ) : (
        <VariableSizeList
          ref={ref}
          width={width}
          itemCount={itemCount}
          itemSize={getItemSize}
          height={calculatedHeight}
          itemData={{
            children: cleanChildren,
            theme: theme,
            activeIndex,
            activedescendant: currentIndex,
            handleMouseMove,
          }}
          outerElementType={CustomScrollbarsVirtualList}
        >
          {Row}
        </VariableSizeList>
      )}
    </>
  );
}

export default VirtualList;
