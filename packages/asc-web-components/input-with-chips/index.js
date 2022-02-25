import React, { useCallback, useEffect, useRef, useState } from "react";
import PropTypes from "prop-types";
import Scrollbar from "../scrollbar";
import { useClickOutside } from "../utils/useClickOutside.js";

import {
  StyledContent,
  StyledChipGroup,
  StyledChipWithInput,
} from "./styled-inputwithchips";
import { tryParseEmail } from "./sub-components/helpers";
import InputGroup from "./sub-components/input-group";
import ChipsRender from "./sub-components/chips-render";

const InputWithChips = ({
  options,
  placeholder,
  onChange,
  clearButtonLabel,
  existEmailText,
  invalidEmailText,
  exceededLimit,
  exceededLimitText,
  maxLength,
  ...props
}) => {
  const [chips, setChips] = useState(options || []);
  const [currentChip, setCurrentChip] = useState(null);
  const [selectedChips, setSelectedChips] = useState([]);

  const containerRef = useRef(null);
  const inputRef = useRef(null);
  const blockRef = useRef(null);
  const scrollbarRef = useRef(null);
  const chipsCount = useRef(options?.length);
  const selectedChipsCount = useRef(0);

  useEffect(() => {
    if (selectedChipsCount.current > 0 || selectedChips.length > 0) {
      onChange(selectedChips);
    }
    selectedChipsCount.current = selectedChips.length;
  }, [selectedChips]);

  useEffect(() => {
    const isChipAdd = chips.length > chipsCount.current;
    if (scrollbarRef.current && isChipAdd) {
      scrollbarRef.current.scrollToBottom();
    }
    chipsCount.current = chips.length;
  }, [chips.length]);

  useClickOutside(
    blockRef,
    () => {
      if (selectedChips.length > 0) {
        setSelectedChips([]);
      }
    },
    selectedChips
  );

  const onClick = (value, isShiftKey) => {
    if (isShiftKey) {
      const isExisted = !!selectedChips?.find((it) => it.value === value.value);
      return isExisted
        ? setSelectedChips(
            selectedChips.filter((it) => it.value != value.value)
          )
        : setSelectedChips([value, ...selectedChips]);
    } else {
      setSelectedChips([value]);
    }
  };

  const onDoubleClick = (value) => {
    setCurrentChip(value);
  };

  const onDelete = useCallback(
    (value) => {
      setChips(chips.filter((it) => it.value !== value.value));
    },
    [chips]
  );

  const checkSelected = (value) => {
    return !!selectedChips?.find((item) => item?.value === value?.value);
  };

  const onSaveNewChip = (value, newValue) => {
    let parsed = tryParseEmail(newValue);
    if (!parsed) {
      if (newValue && newValue !== value.value) {
        const newChips = chips.map((it) => {
          return it.value === value.value
            ? { label: newValue, value: newValue }
            : it;
        });
        setChips(newChips);
        setSelectedChips([{ label: newValue, value: newValue }]);
      }
    } else {
      if (
        parsed.value &&
        (parsed.value !== value.value || parsed.label !== value.label)
      ) {
        const newChips = chips.map((it) => {
          return it.value === value.value ? parsed : it;
        });
        setChips(newChips);
        setSelectedChips([parsed]);
      }
    }

    containerRef.current.setAttribute("tabindex", "-1");
    containerRef.current.focus();

    setCurrentChip(null);
  };

  const copyToClipbord = () => {
    if (currentChip === null) {
      navigator.clipboard.writeText(
        selectedChips
          .map((it) => {
            if (it.label !== it.value) {
              let copyItem = `"${it.label}" <${it.value}>`;
              return copyItem;
            } else {
              return it.value;
            }
          })
          .join(", ")
      );
    }
  };

  const onKeyDown = (e) => {
    const whiteList = [
      "Enter",
      "Escape",
      "Backspace",
      "Delete",
      "ArrowRigth",
      "ArrowLeft",
      "ArrowLeft",
      "ArrowRight",
      "KeyC",
    ];

    const code = e.code;

    const isShiftDown = e.shiftKey;
    const isCtrlDown = e.ctrlKey;

    if (!whiteList.includes(code) && !isCtrlDown && !isShiftDown) {
      return;
    }
    if (code === "Enter" && selectedChips.length == 1 && !currentChip) {
      e.stopPropagation();
      setCurrentChip(selectedChips[0]);
      return;
    }

    if (code === "Escape") {
      setSelectedChips(currentChip ? [currentChip] : []);
      containerRef.current.setAttribute("tabindex", "0");
      containerRef.current.focus();
      return;
    }

    if (
      selectedChips.length > 0 &&
      (code === "Backspace" || code === "Delete") &&
      !currentChip
    ) {
      const filteredChips = chips.filter((e) => !~selectedChips.indexOf(e));
      setChips(filteredChips);
      setSelectedChips([]);
      inputRef.current.focus();
      return;
    }

    if (selectedChips.length > 0 && !currentChip) {
      let chip = null;

      if (isShiftDown && code === "ArrowRigth") {
        chip = selectedChips[selectedChips.length - 1];
      } else {
        chip = selectedChips[0];
      }

      const index = chips.findIndex((it) => it.value === chip?.value);

      switch (code) {
        case "ArrowLeft": {
          if (isShiftDown) {
            selectedChips.includes(chips[index - 1])
              ? setSelectedChips(
                  selectedChips.filter((it) => it !== chips[index])
                )
              : chips[index - 1] &&
                setSelectedChips([chips[index - 1], ...selectedChips]);
          } else if (index != 0) {
            setSelectedChips([chips[index - 1]]);
          }
          break;
        }
        case "ArrowRight": {
          if (isShiftDown) {
            selectedChips.includes(chips[index + 1])
              ? setSelectedChips(
                  selectedChips.filter((it) => it !== chips[index])
                )
              : chips[index + 1] &&
                setSelectedChips([chips[index + 1], ...selectedChips]);
          } else {
            if (index != chips.length - 1) {
              setSelectedChips([chips[index + 1]]);
            } else {
              setSelectedChips([]);
              if (inputRef) {
                inputRef.current.focus();
              }
            }
          }
          break;
        }
        case "KeyC": {
          if (isCtrlDown) {
            copyToClipbord();
          }
          break;
        }
      }
    }
  };

  return (
    <StyledContent {...props}>
      <StyledChipGroup onKeyDown={onKeyDown} ref={containerRef} tabindex="-1">
        <StyledChipWithInput length={chips.length}>
          <Scrollbar scrollclass={"scroll"} stype="thumbV" ref={scrollbarRef}>
            <ChipsRender
              chips={chips}
              checkSelected={checkSelected}
              currentChip={currentChip}
              blockRef={blockRef}
              onClick={onClick}
              invalidEmailText={invalidEmailText}
              onDelete={onDelete}
              onDoubleClick={onDoubleClick}
              onSaveNewChip={onSaveNewChip}
            />
          </Scrollbar>

          <InputGroup
            chips={chips}
            setChips={setChips}
            inputRef={inputRef}
            clearButtonLabel={clearButtonLabel}
            setSelectedChips={setSelectedChips}
            containerRef={containerRef}
            placeholder={placeholder}
            exceededLimit={exceededLimit}
            exceededLimitText={exceededLimitText}
            existEmailText={existEmailText}
            maxLength={maxLength}
          />
        </StyledChipWithInput>
      </StyledChipGroup>
    </StyledContent>
  );
};

InputWithChips.propTypes = {
  /** Array of objects with chips */
  options: PropTypes.arrayOf(PropTypes.object),
  /** Placeholder text for the input */
  placeholder: PropTypes.string,
  /** The text that is displayed in the button for cleaning all chips */
  clearButtonLabel: PropTypes.string,
  /** Warning text when entering an existing email */
  existEmailText: PropTypes.string,
  /** Warning text when entering an invalid email */
  invalidEmailText: PropTypes.string,
  /** Limit of chips */
  exceededLimit: PropTypes.number,
  /** Warning text when entering the number of chips exceeding the limit */
  exceededLimitText: PropTypes.string,
  /** The number of allowed characters in input */
  maxLength: PropTypes.number,
  /** Will be called when the selected items are changed */
  onChange: PropTypes.func.isRequired,
};

InputWithChips.defaultProps = {
  placeholder: "Invite people by name or email",
  clearButtonLabel: "Clear list",
  existEmailText: "This email address has already been entered",
  invalidEmailText: "Invalid email address",
  exceededLimitText:
    "The limit on the number of emails has reached the maximum",
  exceededLimit: 50,
  maxLength: 200,
};

export default InputWithChips;
