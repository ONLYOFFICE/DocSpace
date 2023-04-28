import React, { useCallback, useEffect, useRef, useState } from "react";
import PropTypes from "prop-types";
import Scrollbar from "../scrollbar";
import { useClickOutside } from "../utils/useClickOutside.js";

import {
  StyledContent,
  StyledChipGroup,
  StyledChipWithInput,
} from "./styled-emailchips";
import {
  MAX_EMAIL_LENGTH_WITH_DOTS,
  sliceEmail,
} from "./sub-components/helpers";
import InputGroup from "./sub-components/input-group";
import ChipsRender from "./sub-components/chips-render";
import { EmailSettings, parseAddresses } from "../utils/email";

const calcMaxLengthInput = (exceededLimit) =>
  exceededLimit * MAX_EMAIL_LENGTH_WITH_DOTS;

const EmailChips = ({
  options,
  placeholder,
  onChange,
  clearButtonLabel,
  existEmailText,
  invalidEmailText,
  exceededLimit,
  exceededLimitText,
  exceededLimitInputText,
  chipOverLimitText,
  ...props
}) => {
  const [chips, setChips] = useState(options || []);
  const [currentChip, setCurrentChip] = useState(null);
  const [selectedChips, setSelectedChips] = useState([]);

  const [isExistedOn, setIsExistedOn] = useState(false);
  const [isExceededLimitChips, setIsExceededLimitChips] = useState(false);
  const [isExceededLimitInput, setIsExceededLimitInput] = useState(false);

  const containerRef = useRef(null);
  const inputRef = useRef(null);
  const blockRef = useRef(null);
  const scrollbarRef = useRef(null);
  const chipsCount = useRef(options?.length);

  useEffect(() => {
    onChange(
      chips.map((it) => {
        if (it?.name === it?.email || it?.name === "") {
          return {
            email: it?.email,
            isValid: it?.isValid,
          };
        }
        return {
          name: it?.name,
          email: it?.email,
          isValid: it?.isValid,
        };
      })
    );
  }, [chips]);

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

  useClickOutside(inputRef, () => {
    onHideAllTooltips();
  });

  const onClick = (value, isShiftKey) => {
    if (isShiftKey) {
      const isExisted = !!selectedChips?.find((it) => it.email === value.email);
      return isExisted
        ? setSelectedChips(
            selectedChips.filter((it) => it.email != value.email)
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
      setChips(chips.filter((it) => it.email !== value.email));
    },
    [chips]
  );

  const checkSelected = (value) => {
    return !!selectedChips?.find((item) => item?.email === value?.email);
  };

  const onSaveNewChip = (value, newValue) => {
    const settings = new EmailSettings();
    settings.allowName = true;
    let parsed = parseAddresses(newValue, settings);
    parsed[0].isValid = parsed[0].isValid();
    if (newValue && newValue !== `"${value?.name}" <${value?.email}>`) {
      const newChips = chips.map((it) => {
        return it.email === value.email ? sliceEmail(parsed[0]) : it;
      });
      setChips(newChips);
      setSelectedChips([sliceEmail(parsed[0])]);
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
            if (it.name !== it.email) {
              let copyItem = `"${it.name}" <${it.email}>`;
              return copyItem;
            } else {
              return it.email;
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

      const index = chips.findIndex((it) => it.email === chip?.email);

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

  const goFromInputToChips = () => {
    setSelectedChips([chips[chips?.length - 1]]);
  };

  const onClearClick = () => {
    setChips([]);
  };

  const onHideAllTooltips = () => {
    setIsExceededLimitChips(false);
    setIsExistedOn(false);
    setIsExceededLimitInput(false);
  };

  const showTooltipOfLimit = () => {
    setIsExceededLimitInput(true);
  };

  const onAddChip = (chipsToAdd) => {
    setIsExceededLimitChips(chips.length >= exceededLimit);
    if (chips.length >= exceededLimit) return;
    const filterLimit = exceededLimit - chips.length;

    const filteredChips = chipsToAdd.map(sliceEmail).filter((it, index) => {
      const isExisted = !!chips.find(
        (chip) => chip.email === it || chip.email === it?.email
      );
      if (chipsToAdd.length === 1) {
        setIsExistedOn(isExisted);
        if (isExisted) return false;
      }
      return !isExisted && index < filterLimit;
    });
    setChips([...chips, ...filteredChips]);
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
              chipOverLimitText={chipOverLimitText}
              onDelete={onDelete}
              onDoubleClick={onDoubleClick}
              onSaveNewChip={onSaveNewChip}
            />
          </Scrollbar>

          <InputGroup
            placeholder={placeholder}
            exceededLimitText={exceededLimitText}
            existEmailText={existEmailText}
            exceededLimitInputText={exceededLimitInputText}
            clearButtonLabel={clearButtonLabel}
            inputRef={inputRef}
            containerRef={containerRef}
            maxLength={calcMaxLengthInput(exceededLimit)}
            goFromInputToChips={goFromInputToChips}
            onClearClick={onClearClick}
            isExistedOn={isExistedOn}
            isExceededLimitChips={isExceededLimitChips}
            isExceededLimitInput={isExceededLimitInput}
            onHideAllTooltips={onHideAllTooltips}
            showTooltipOfLimit={showTooltipOfLimit}
            onAddChip={onAddChip}
          />
        </StyledChipWithInput>
      </StyledChipGroup>
    </StyledContent>
  );
};

EmailChips.propTypes = {
  /** Array of objects with chips */
  options: PropTypes.arrayOf(PropTypes.object),
  /** Placeholder text for the input */
  placeholder: PropTypes.string,
  /** The text displayed in the button that triggers cleaning all chips */
  clearButtonLabel: PropTypes.string,
  /** Warning text when entering an existing email */
  existEmailText: PropTypes.string,
  /** Warning text when entering an invalid email */
  invalidEmailText: PropTypes.string,
  /** Limit of chips */
  exceededLimit: PropTypes.number,
  /** Warning text when entering the number of chips exceeding the limit */
  exceededLimitText: PropTypes.string,
  /** Warning text when the number of inputted characters exceeds the limit */
  exceededLimitInputText: PropTypes.string,
  /** Warning text when the number of the email address characters exceeds the limit */
  chipOverLimitText: PropTypes.string,
  /** Sets a callback function that will be called when the selected items are changed */
  onChange: PropTypes.func.isRequired,
};

EmailChips.defaultProps = {
  placeholder: "Invite people by name or email",
  clearButtonLabel: "Clear list",
  existEmailText: "This email address has already been entered",
  invalidEmailText: "Invalid email address",
  exceededLimitText:
    "The limit on the number of emails has reached the maximum",
  exceededLimitInputText:
    "The limit on the number of characters has reached the maximum value",
  exceededLimit: 50,
};

export default EmailChips;
