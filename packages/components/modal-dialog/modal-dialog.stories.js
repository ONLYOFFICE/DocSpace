import React, { useState } from "react";
import ModalDialog from "./index.js";
import Button from "../button";
import PropTypes from "prop-types";

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
    onOk: { action: "onOk" },
  },
};

const Template = ({ onOk, ...args }) => {
  const [isVisible, setIsVisible] = useState(false);

  const openModal = () => setIsVisible(true);
  const closeModal = () => setIsVisible(false);

  return (
    <>
      <Button label="Show" primary={true} size="medium" onClick={openModal} />
      <ModalDialog {...args} visible={isVisible} onClose={closeModal}>
        <ModalDialog.Header>Change password</ModalDialog.Header>
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
            size="normal"
            onClick={() => {
              onOk();
              closeModal();
            }}
          />
          <Button
            key="CloseBtn"
            label="Cancel"
            scale
            size="normal"
            onClick={closeModal}
          />
        </ModalDialog.Footer>
      </ModalDialog>
    </>
  );
};

Template.propTypes = {
  onOk: PropTypes.func,
};

export const Default = Template.bind({});
Default.args = {
  displayType: "aside",
  displayTypeDetailed: {
    desktop: "modal",
    tablet: "aside",
    smallTablet: "modal",
    mobile: "aside",
  },
};
