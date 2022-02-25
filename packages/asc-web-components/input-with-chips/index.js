import React, { useCallback, useEffect, useRef, useState } from "react";
import PropTypes from "prop-types";

import Chip from "./sub-components/chip";
import TextInput from "../text-input";
import Scrollbar from "../scrollbar";
import { EmailSettings, parseAddress } from "../utils/email/";
import { useClickOutside } from "../utils/useClickOutside.js";
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
  clearButtonLabel,
  existEmailText,
  invalidEmailText,
  exceededLimit,
  ...props
}) => {
  const [chips, setChips] = useState(options || []);
  const [currentChip, setCurrentChip] = useState(null);
  const [selectedChips, setSelectedChips] = useState([]);

  const [value, setValue] = useState("");

  const [isExistedOn, setIsExistedOn] = useState(false);
  const [isExceededLimit, setIsExceededLimit] = useState(false);

  const emailSettings = new EmailSettings();

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

  useClickOutside(blockRef, () => {
    setSelectedChips([]);
    setIsExistedOn(false);
    setIsExceededLimit(false);
  });

  const onInputChange = (e) => {
    setValue(e.target.value);
    hideTooltipOnEmptyInput(e);
  };

  const hideTooltipOnEmptyInput = (e) => {
    if (e.target.value == "") setIsExceededLimit(false);
  };

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
      setIsExceededLimit(false);
    },
    [chips]
  );

  const tryParseEmail = useCallback(
    (emailString) => {
      const cortege = emailString
        .trim()
        .split('" <')
        .map((it) => it.trim());

      if (cortege.length != 2) return false;
      let label = cortege[0];
      if (label[0] != '"') return false;
      label = label.slice(1);

      let email = cortege[1];
      if (email[email.length - 1] != ">") return false;
      email = email.slice(0, -1);

      return { label, value: email };
    },
    [onEnterPress]
  );

  const onEnterPress = () => {
    if (chips.length < 2000) {
      if (value.trim().length > 0) {
        const separators = [",", " ", ", "];
        let indexesForFilter = [];
        const chipsFromString = value
          .split(new RegExp(separators.join("|"), "g"))
          .filter((it) => it.trim().length !== 0)
          .map((it, idx, arr) => {
            if (it.includes('"') && arr[idx + 1]) {
              indexesForFilter.push(idx + 1);
              return `${it} ${arr[idx + 1]}`;
            }
            return it;
          })
          .map((it) => (tryParseEmail(it) ? tryParseEmail(it) : it.trim()))
          .filter((it, idx) => !indexesForFilter.includes(idx));
        if (chipsFromString.length === 1) {
          let isExisted = !!chips.find(
            (chip) =>
              chip.value === chipsFromString[0] ||
              chip.value === chipsFromString[0]?.value
          );
          setIsExistedOn(isExisted);
          if (isExisted) return;
        }

        const filteredChips = chipsFromString
          .filter((it) => {
            return !chips.find(
              (chip) => chip.value === it || chip.value === it?.value
            );
          })
          .map((it) => ({ label: it?.label ?? it, value: it?.value ?? it }));
        setChips([...chips, ...filteredChips]);
        setValue("");
      }
    } else {
      setIsExceededLimit(true);
    }
  };

  const checkEmail = useCallback(
    (email) => {
      const emailObj = parseAddress(email, emailSettings);
      return emailObj.isValid();
    },
    [onEnterPress]
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

  const onClearList = () => {
    setChips([]);
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
      setIsExceededLimit(false);
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

  const renderChips = () => {
    return (
      <StyledAllChips ref={blockRef}>
        {chips?.map((it) => {
          return (
            <Chip
              key={it?.value}
              value={it}
              currentChip={currentChip}
              isSelected={checkSelected(it)}
              isValid={checkEmail(it?.value)}
              invalidEmailText={invalidEmailText}
              onDelete={onDelete}
              onDoubleClick={onDoubleClick}
              onSaveNewChip={onSaveNewChip}
              onClick={onClick}
              setSelectedChips={setSelectedChips}
            />
          );
        })}
      </StyledAllChips>
    );
  };

  const renderInput = () => {
    return (
      <TextInput
        value={value}
        onChange={onInputChange}
        forwardedRef={inputRef}
        onKeyDown={onInputKeyDown}
        placeholder={placeholder}
        withBorder={false}
        className="textInput"
        maxLength={2000}
        chips={chips.length}
      />
    );
  };

  return (
    <StyledContent {...props}>
      <StyledChipGroup onKeyDown={onKeyDown} ref={containerRef} tabindex="-1">
        <StyledChipWithInput length={chips.length}>
          <Scrollbar scrollclass={"scroll"} stype="thumbV" ref={scrollbarRef}>
            {renderChips()}
          </Scrollbar>
          <StyledInputWithLink>
            {isExistedOn && <StyledTooltip>{existEmailText}</StyledTooltip>}
            {isExceededLimit && <StyledTooltip>{exceededLimit}</StyledTooltip>}
            {renderInput()}
            <Link
              type="action"
              isHovered={true}
              className="link"
              onClick={onClearList}
            >
              {clearButtonLabel}
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
  /** The text that is displayed in the button for cleaning all chips */
  clearButtonLabel: PropTypes.string,
  /** Warning text when entering an existing email */
  existEmailText: PropTypes.string,
  /** Warning text when entering an invalid email */
  invalidEmailText: PropTypes.string,
  /** Warning text when entering the number of chips exceeding the limit */
  exceededLimit: PropTypes.string,
  /** Will be called when the selected items are changed */
  onChange: PropTypes.func.isRequired,
};

InputWithChips.defaultProps = {
  placeholder: "Invite people by name or email",
  existEmailText: PropTypes.string,
  invalidEmailText: PropTypes.string,
};

export default InputWithChips;
