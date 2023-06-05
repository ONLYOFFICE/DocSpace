import React from "react";
import AvatarEditorComponent from "./";
import Avatar from "../avatar";

export default {
  title: "Components/AvatarEditor",
  component: AvatarEditorComponent,
  argTypes: {
    openEditor: { action: "onOpen", table: { disable: true } },
    closeEditor: { action: "onClose", table: { disable: true } },
    onSave: { action: "onSave", table: { disable: true } },
    onLoadFile: { action: "onLoadFile", table: { disable: true } },
    onImageChange: { action: "onImageChange", table: { disable: true } },
    onDeleteImage: { action: "onDeleteImage", table: { disable: true } },
  },
  parameters: {
    docs: {
      description: {
        component: "Used to display user avatar editor on page.",
      },
    },
  },
};

class AvatarEditor extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      isOpen: false,
      userImage: null,
    };

    this.openEditor = this.openEditor.bind(this);
    this.onClose = this.onClose.bind(this);
    this.onSave = this.onSave.bind(this);
    this.onLoadFile = this.onLoadFile.bind(this);
    this.onImageChange = this.onImageChange.bind(this);
    this.onDeleteImage = this.onDeleteImage.bind(this);
  }

  onDeleteImage() {
    this.props.onDeleteImage();
  }
  onImageChange(img) {
    this.props.onImageChange(img);
    this.setState({
      userImage: img,
    });
  }
  onLoadFile(file) {
    this.props.onLoadFile(file);
  }
  onSave(isUpdate, data) {
    this.props.onSave(isUpdate, data);
    this.setState({
      isOpen: false,
    });
  }
  openEditor(e) {
    this.props.openEditor(e);
    this.setState({
      isOpen: true,
    });
  }
  onClose() {
    this.props.closeEditor();
    this.setState({
      isOpen: false,
    });
  }
  render() {
    const {
      unknownError,
      unknownTypeError,
      saveButtonLoading,
      maxSizeFileError,
    } = this.props;
    return (
      <>
        <Avatar
          size="max"
          role="user"
          source={this.state.userImage}
          editing={true}
          editAction={this.openEditor}
        />
        {this.props.children}
        <AvatarEditorComponent
          {...this.props}
          visible={this.state.isOpen || this.props.visible}
          onClose={this.onClose}
          onSave={this.onSave}
          onCancel={this.onClose}
          onDeleteImage={this.onDeleteImage}
          onImageChange={this.onImageChange}
          onLoadFile={this.onLoadFile}
          chooseFileLabel={"Drop files here, or click to select files"}
          chooseMobileFileLabel={"Click to select files"}
          saveButtonLoading={saveButtonLoading}
          maxSizeFileError={maxSizeFileError || "Maximum file size exceeded"}
          unknownTypeError={unknownTypeError || "Unknown image file type"}
          unknownError={unknownError || "Error"}
        />
      </>
    );
  }
}
const Template = (args) => {
  return <AvatarEditor {...args} />;
};
export const Default = Template.bind({});
