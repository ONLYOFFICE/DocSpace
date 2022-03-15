import React, { useState } from "react";
import ModalDialog from ".";
import Button from "../button";

export default {
  title: "Components/ModalDialog",
  component: ModalDialog,
  parameters: {
    docs: {
      description: {
        component: "ModalDialog is used for displaying modal dialogs",
      },
    },
  },
  argTypes: {
    onClick: { action: "onClick" },
    onClose: { action: "onClose" },
    onOk: { action: "onOk", table: { disable: true } },
  },
};

const Template = ({ onClick, onClose, onOk, ...args }) => {
  const [isVisible, setIsVisible] = useState(args.visible);

  const toggleVisible = (e) => {
    setIsVisible(!isVisible);
    onClick(e);
  };

  return (
    <div>
      <Button
        label="Show"
        primary={true}
        size="medium"
        onClick={toggleVisible}
      />
      <ModalDialog {...args} visible={isVisible} onClose={toggleVisible}>
        <ModalDialog.Header>{"Change password"}</ModalDialog.Header>
        <ModalDialog.Body>
          <span>
            Send the password change instruction to the{" "}
            <a href="mailto:asc@story.book">asc@story.book</a> email address
          </span>
        </ModalDialog.Body>
        <ModalDialog.Footer>
          <Button
            key="SendBtn"
            label="Send"
            primary={true}
            scale
            size="big"
            onClick={(e) => {
              toggleVisible(e);
              onOk(e);
            }}
          />
          <Button
            key="CloseBtn"
            label="Cancel"
            scale
            size="normal"
            onClick={toggleVisible}
          />
        </ModalDialog.Footer>
      </ModalDialog>
    </div>
  );
};

export const Default = Template.bind({});
Default.args = {
  scale: false,
  displayType: "aside",
  zIndex: 310,
  headerContent: "Change password",
};
