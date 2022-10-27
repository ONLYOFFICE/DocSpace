import ItemIcon from "./index";

export default {
  title: "Components/ItemIcon",
  component: ItemIcon,
};

const RoomIconTemplate = ({ onChange, value, ...args }) => {
  return (
    <div style={{ height: "120px" }}>
      <ItemIcon {...args} />
    </div>
  );
};
export const RoomIcon = RoomIconTemplate.bind({});

const FileIconTemplate = ({ onChange, value, ...args }) => {
  return (
    <div style={{ height: "120px" }}>
      <ItemIcon {...args} />
    </div>
  );
};
export const FileIcon = FileIconTemplate.bind({});

RoomIcon.args = {
  item: {
    icon: "images/icons/32/room/filling.form.svg",
    logo: {
      small: "",
      medium: "",
      large: "",
      original: "",
    },
    isRoom: true,
    roomType: 1,
    isPrivate: false,
    isArchive: true,
  },
  roomLogoSize: "large",
};

FileIcon.args = {
  item: {
    href: "/doceditor?fileId=2",
    icon: "/static/images/icons/32/docx.svg",
    fileExst: undefined,
  },
};
