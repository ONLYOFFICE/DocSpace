import React from "react";
import AvatarEditor from "./";
import Avatar from "../avatar";

export default {
  title: "Components/AvatarEditor",
  component: AvatarEditor,
  argTypes: {
    visible: { description: "Display avatar editor" },
    image: { description: "Display avatar editor" },
    accept: { description: "Accepted file types" },
    displayType: { description: "Display type" },
    useModalDialog: {
      description: "Use for the view of the modal dialog or not",
    },
    selectNewPhotoLabel: {
      description: "Translation string for file selection",
    },
    orDropFileHereLabel: {
      description:
        "Translation string for file dropping (concat with selectNewPhotoLabel prop)",
    },
    headerLabel: { description: "Translation string for title" },
    saveButtonLabel: { description: "Translation string for save button" },
    saveButtonLoading: {
      description: "Tells when the button should show loader icon",
    },
    cancelButtonLabel: { description: "Translation string for cancel button" },
    maxSizeFileError: { description: "Translation string for size warning" },
    unknownTypeError: {
      description: "Translation string for file type warning",
    },
    onLoadFileError: {
      description: "Translation string for load file warning",
    },
    unknownError: { description: "Translation string for warning" },
    maxSize: { description: "Max size of image" },
    onSave: { description: "Save event" },
    onClose: { description: "Closing event " },
    onDeleteImage: { description: "Image deletion event" },
    onLoadFile: { description: "Image upload event" },
    onImageChange: { description: "Image change event" },
    className: { description: "Accepts class" },
    id: { description: "Accepts id" },
    style: { description: "Accepts css style" },
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
      source: {
        code: `
        import AvatarEditor from "@appserver/components/avatar-editor";

<AvatarEditor
  accept={[
    'image/png',
    'image/jpeg'
  ]}
  onClose={() => {}}
  onDeleteImage={function noRefCheck() {}}
  onImageChange={function noRefCheck() {}}
  onLoadFile={function noRefCheck() {}}
  onLoadFileError={function noRefCheck() {}}
  onSave={function noRefCheck() {}}
/>
        `,
      },
    },
  },
};

class Wrapper extends React.Component {
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
    console.log(this.props);
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
        <AvatarEditor
          {...this.props}
          visible={this.state.isOpen || this.props.visible}
          onClose={this.onClose}
          onSave={this.onSave}
          onDeleteImage={this.onDeleteImage}
          onImageChange={this.onImageChange}
          onLoadFile={this.onLoadFile}
          //headerLabel={"Edit Photo"}
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
  return <Wrapper {...args} />;
};
export const Default = Template.bind({});

/*import { storiesOf } from "@storybook/react";
import { action } from "@storybook/addon-actions";
import { withKnobs, text, select } from "@storybook/addon-knobs/react";
import withReadme from "storybook-readme/with-readme";
import Readme from "./README.md";
import AvatarEditor from ".";
import Avatar from "../avatar";
import Section from "../../../.storybook/decorators/section";
import { boolean } from "@storybook/addon-knobs";

const displayType = ["auto", "modal", "aside"];
class AvatarEditorStory extends React.Component {
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
    this.onLoadFile = this.onLoadFile.bind(this);
  }
  onDeleteImage() {
    action("onDeleteImage");
  }
  onImageChange(img) {
    action("onLoadFile");
    this.setState({
      userImage: img,
    });
  }
  onLoadFile(file) {
    action("onLoadFile")(file);
  }
  onSave(isUpdate, data) {
    action("onSave")(isUpdate, data);
    this.setState({
      isOpen: false,
    });
  }
  openEditor() {
    this.setState({
      isOpen: true,
    });
  }
  onClose() {
    action("onClose");
    this.setState({
      isOpen: false,
    });
  }
  render() {
    return (
      <div>
        <Avatar
          size="max"
          role="user"
          source={this.state.userImage}
          editing={true}
          editAction={this.openEditor}
        />
        <AvatarEditor
          visible={this.state.isOpen}
          onClose={this.onClose}
          onSave={this.onSave}
          onDeleteImage={this.onDeleteImage}
          onImageChange={this.onImageChange}
          onLoadFile={this.onLoadFile}
          headerLabel={text("headerLabel", "Edit Photo")}
          chooseFileLabel={text(
            "chooseFileLabel",
            "Drop files here, or click to select files"
          )}
          chooseMobileFileLabel={text(
            "chooseMobileFileLabel",
            "Click to select files"
          )}
          saveButtonLabel={text("saveButtonLabel", "Save")}
          saveButtonLoading={boolean("saveButtonLoading", false)}
          maxSizeFileError={text(
            "maxSizeFileError",
            "Maximum file size exceeded"
          )}
          unknownTypeError={text("unknownTypeError", "Unknown image file type")}
          unknownError={text("unknownError", "Error")}
          displayType={select("displayType", displayType, "auto")}
        />
      </div>
    );
  }
}

storiesOf("Components|AvatarEditor", module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add("base", () => {
    return (
      <Section>
        <AvatarEditorStory />
      </Section>
    );
  });
*/
