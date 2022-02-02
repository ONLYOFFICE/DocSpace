import React, { useEffect, useState } from "react";
import PropTypes from "prop-types";

import Chip from "./sub-components/chip";
import TextInput from "../text-input";
import Scrollbar from "../scrollbar";
import { EmailSettings, parseAddress } from "../utils/email/";

import {
  StyledContent,
  StyledChipGroup,
  StyledChipWithInput,
} from "./styled-inputwithchips";

const InputWithChips = (props) => {
  const [chips, setChips] = useState(props.chips || []);

  const [value, setValue] = useState("");

  const [currentChip, setCurrentChip] = useState(null);
  const [selectedChips, setSelectedChips] = useState([]);

  const [isShiftDown, setIsShiftDown] = useState(false);
  const [isCtrlDown, setIsCtrlDown] = useState(false);

  const emailSettings = new EmailSettings();

  useEffect(() => {
    document.addEventListener("keydown", onKeyDown);
    document.addEventListener("keyup", onKeyUp);
    return () => {
      document.removeEventListener("keydown", onKeyDown);
      document.removeEventListener("keyup", onKeyUp);
    };
  }, [selectedChips, isShiftDown, isCtrlDown]);

  const onChange = (e) => {
    setValue(e.target.value);
  };

  const onClick = (value) => {
    setSelectedChips([value]);
  };

  const onDoubleClick = (value) => {
    setCurrentChip(value);
  };

  const onDelete = (value) => {
    setChips(chips.filter((it) => it.value !== value.value));
  };

  const onEnterPress = () => {
    if (!chips.find((it) => it.value === value) && value) {
      setChips([...chips, { label: value, value }]);
      setValue("");
    }
  };

  const checkEmail = (email) => {
    const emailObj = parseAddress(email, emailSettings);
    return emailObj.isValid();
  };

  const onSaveNewChip = (value, newValue) => {
    setChips(
      chips.map((it) =>
        it.value === value.value ? { label: newValue, value: newValue } : it
      )
    );
  };

  const onInputKeyDown = (e) => {
    const code = e.code;
    if (code === "Enter" || code === "NumpadEnter") onEnterPress();
  };

  const copyToClipbord = () => {
    navigator.clipboard.writeText(selectedChips.map((it) => it.value).join());
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

    if (selectedChips.length > 1 && code === "Backspace") {
      const Chips = chips.filter((e) => !~selectedChips.indexOf(e));
      setChips(Chips);
      setSelectedChips([]);
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
          } else {
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
            setSelectedChips([chips[index + 1]]);
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
        <Scrollbar
          scrollclass={"scroll"}
          stype="thumbV"
          style={{ height: "fit-content" }}
        >
          <StyledChipWithInput>
            {chips.map((it) => {
              return (
                <Chip
                  key={it?.value}
                  value={it}
                  currentChip={currentChip}
                  isSelected={
                    !!selectedChips.find((item) => item?.value === it?.value)
                  }
                  isValid={checkEmail(it?.value)}
                  onDelete={onDelete}
                  onDoubleClick={onDoubleClick}
                  onSaveNewChip={onSaveNewChip}
                  onClick={onClick}
                />
              );
            })}
            <TextInput
              value={value}
              onChange={onChange}
              placeholder={chips.length > 0 ? "" : props.placeholder}
              onKeyDown={onInputKeyDown}
              withBorder={false}
            />
          </StyledChipWithInput>
        </Scrollbar>
      </StyledChipGroup>
    </StyledContent>
  );
};

InputWithChips.propTypes = {
  chips: PropTypes.arrayOf(PropTypes.object),
  max: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
  maxlength: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
  placeholder: PropTypes.string,
};

export default InputWithChips;
