import React, { useCallback, useEffect, useRef, useState } from "react";
import PropTypes from "prop-types";

import Chip from "./sub-components/chip";
import TextInput from "../text-input";
import Scrollbar from "../scrollbar";
import { EmailSettings, parseAddress } from "../utils/email/";
import { useClickOutside } from "./sub-components/use-click-outside";
import Link from "../link";

import {
  StyledContent,
  StyledChipGroup,
  StyledChipWithInput,
  StyledAllChips,
  StyledInputWithLink,
  StyledTooltip,
} from "./styled-inputwithchips";

const InputWithChips = ({
  options,
  placeholder,
  onChange,
  existEmailText = "This email address has already been entered",
  invalidEmailText,
  ...props
}) => {
  const [chips, setChips] = useState(options || []);
  const [currentChip, setCurrentChip] = useState(null);
  const [selectedChips, setSelectedChips] = useState([]);

  const [value, setValue] = useState("");

  const [isExistedOn, setIsExistedOn] = useState(false);

  const emailSettings = new EmailSettings();

  const containerRef = useRef(null);
  const inputRef = useRef(null);
  const blockRef = useRef(null);
  const scrollbarRef = useRef(null);
  const chipsCount = useRef(options.length);
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

  useClickOutside(blockRef, () => {
    setSelectedChips([]);
    setIsExistedOn(false);
  });

  const onInputChange = (e) => {
    setValue(e.target.value);
  };

  const onClick = useCallback(
    (value, isShiftKey) => {
      if (isShiftKey) {
        const isExisted = !!selectedChips?.find(
          (it) => it.value === value.value
        );
        return isExisted
          ? setSelectedChips(
              selectedChips.filter((it) => it.value != value.value)
            )
          : setSelectedChips([value, ...selectedChips]);
      } else {
        setSelectedChips([value]);
      }
    },
    [selectedChips]
  );

  const onDoubleClick = useCallback(
    (value) => {
      setSelectedChips([]);
      setCurrentChip(value);
    },
    [value]
  );

  const onDelete = useCallback(
    (value) => {
      setChips(chips.filter((it) => it.value !== value.value));
    },
    [chips]
  );

  const onEnterPress = () => {
    if (value.trim().length > 0) {
      const separators = [",", " ", ", "];
      const chipsFromString = value
        .split(new RegExp(separators.join("|"), "g"))
        .filter((it) => it.trim().length !== 0);

      if (chipsFromString.length === 1) {
        let isExisted = !!chips.find(
          (chip) => chip.value === chipsFromString[0]
        );
        setIsExistedOn(isExisted);
        if (isExisted) return;
      }

      const filteredChips = chipsFromString
        .filter((it) => {
          return !chips.find((chip) => chip.value === it);
        })
        .map((it) => ({ label: it, value: it }));

      setChips([...chips, ...filteredChips]);
      setValue("");
    }
  };

  const checkEmail = (email) => {
    const emailObj = parseAddress(email, emailSettings);
    return emailObj.isValid();
  };

  const checkSelected = (value) => {
    return !!selectedChips?.find((item) => item?.value === value?.value);
  };

  const onSaveNewChip = useCallback(
    (value, newValue) => {
      if (newValue && newValue !== value.value) {
        setChips(
          chips.map((it) =>
            it.value === value.value ? { label: newValue, value: newValue } : it
          )
        );
      }
    },
    [chips]
  );

  const onInputKeyDown = (e) => {
    const code = e.code;
    const isCursorStart = inputRef.current.selectionStart === 0;
    if (code === "Enter" || code === "NumpadEnter") onEnterPress();
    if (code === "ArrowLeft" && isCursorStart) {
      setSelectedChips([chips[chips.length - 1]]);
      if (inputRef) {
        inputRef.current.blur();
        containerRef.current.setAttribute("tabindex", "0");
        containerRef.current.focus();
      }
    }
  };

  const copyToClipbord = () => {
    if (currentChip === null) {
      navigator.clipboard.writeText(
        selectedChips.map((it) => it.value).join(", ")
      );
    }
  };

  const onClearList = () => {
    setChips([]);
  };

  const onKeyDown = (e) => {
    const code = e.code;
    const isShiftDown = e.shiftKey;
    const isCtrlDown = e.ctrlKey;

    if (code === "Escape") {
      setSelectedChips([]);
      return;
    }

    if (selectedChips.length > 0 && code === "Backspace" && !currentChip) {
      const filteredChips = chips.filter((e) => !~selectedChips.indexOf(e));
      setChips(filteredChips);
      setSelectedChips([]);
      inputRef.current.focus();
      return;
    }

    if (selectedChips.length > 0) {
      let chip = "";

      if (isShiftDown && code === "ArrowRigth") {
        chip = selectedChips[selectedChips.length - 1];
      } else {
        chip = selectedChips[0];
      }

      const index = chips.findIndex((it) => it === chip);
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
    <StyledContent>
      <StyledChipGroup onKeyDown={onKeyDown} ref={containerRef} tabindex="-1">
        <StyledChipWithInput length={chips.length}>
          <Scrollbar scrollclass={"scroll"} stype="thumbV" ref={scrollbarRef}>
            <StyledAllChips ref={blockRef}>
              {chips?.map((it) => {
                return (
                  <Chip
                    key={it?.value}
                    value={it}
                    currentChip={currentChip}
                    isSelected={checkSelected(it)}
                    isValid={checkEmail(it?.value)}
                    onDelete={onDelete}
                    onDoubleClick={onDoubleClick}
                    onSaveNewChip={onSaveNewChip}
                    onClick={onClick}
                    invalidEmailText={invalidEmailText}
                  />
                );
              })}
            </StyledAllChips>
          </Scrollbar>
          <StyledInputWithLink>
            {isExistedOn && <StyledTooltip>{existEmailText}</StyledTooltip>}
            <TextInput
              value={value}
              onChange={onInputChange}
              forwardedRef={inputRef}
              onKeyDown={onInputKeyDown}
              placeholder={placeholder}
              withBorder={false}
              className="textInput"
              chips={chips.length}
            />
            <Link
              type="action"
              isHovered={true}
              className="link"
              onClick={onClearList}
            >
              Clear list
            </Link>
          </StyledInputWithLink>
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
  /** Warning text when entering an existing email */
  existEmailText: PropTypes.string,
  /** Warning text when entering an invalid email */
  invalidEmailText: PropTypes.string,
  /** Will be called when the selected items are changed */
  onChange: PropTypes.func.isRequired,
};

InputWithChips.defaultProps = {
  placeholder: "Invite people by name or email",
};

export default InputWithChips;
