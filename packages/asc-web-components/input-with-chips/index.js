import React, { useEffect, useRef, useState } from "react";
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

  const [isShiftDown, setIsShiftDown] = useState(false);
  const [isCtrlDown, setIsCtrlDown] = useState(false);

  const [isExistedOn, setIsExistedOn] = useState(false);

  const emailSettings = new EmailSettings();

  const inputRef = useRef(null);
  const blockRef = useRef(null);
  const scrollbarRef = useRef(null);

  useEffect(() => {
    document.addEventListener("keydown", onKeyDown);
    document.addEventListener("keyup", onKeyUp);
    return () => {
      document.removeEventListener("keydown", onKeyDown);
      document.removeEventListener("keyup", onKeyUp);
    };
  }, [selectedChips, currentChip, isShiftDown, isCtrlDown]);

  useEffect(() => {
    onChange(selectedChips);
  }, [selectedChips]);

  useClickOutside(blockRef, () => {
    setSelectedChips([]);
    setIsExistedOn(false);
  });

  const onInputChange = (e) => {
    setValue(e.target.value);
  };

  const onClick = (value) => {
    if (isShiftDown) {
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
    setSelectedChips([]);
    setCurrentChip(value);
  };

  const onDelete = (value) => {
    setChips(chips.filter((it) => it.value !== value.value));
  };

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

      if (scrollbarRef.current) {
        scrollbarRef.current.scrollToBottom();
      }
    }
  };

  const checkEmail = (email) => {
    const emailObj = parseAddress(email, emailSettings);
    return emailObj.isValid();
  };

  const checkSelected = (value) => {
    return !!selectedChips?.find((item) => item?.value === value?.value);
  };

  const onSaveNewChip = (value, newValue) => {
    if (newValue && newValue !== value.value) {
      setChips(
        chips.map((it) =>
          it.value === value.value ? { label: newValue, value: newValue } : it
        )
      );
    }
  };

  const onInputKeyDown = (e) => {
    e.stopPropagation();
    const code = e.code;
    const isCursorStart = inputRef.current.selectionStart === 0;
    if (code === "Enter" || code === "NumpadEnter") onEnterPress();
    if (code === "ArrowLeft" && isCursorStart) {
      setSelectedChips([chips[chips.length - 1]]);
      if (inputRef) {
        inputRef.current.blur();
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

  const onKeyUp = (e) => {
    const code = e.code;
    switch (code) {
      case "ShiftLeft": {
        setIsShiftDown(false);
        break;
      }
      case "ControlLeft": {
        setIsCtrlDown(false);
        break;
      }
    }
  };

  const onKeyDown = (e) => {
    const code = e.code;

    if (code === "ShiftLeft") {
      setIsShiftDown(true);
      return;
    }
    if (code === "ControlLeft") {
      setIsCtrlDown(true);
      return;
    }

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
      <StyledChipGroup>
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
  options: PropTypes.arrayOf(PropTypes.object).isRequired,
  /** The placeholder is displayed only when the input is empty */
  placeholder: PropTypes.string,
  existEmailText: PropTypes.string,
  invalidEmailText: PropTypes.string,
  onChange: PropTypes.func,
};

InputWithChips.defaultProps = {
  placeholder: "Add placeholder to props",
};

export default InputWithChips;
