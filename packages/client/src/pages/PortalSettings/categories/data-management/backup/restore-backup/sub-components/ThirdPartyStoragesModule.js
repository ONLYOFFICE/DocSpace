import AmazonStorage from "./storages/AmazonStorage";
import RackspaceStorage from "./storages/RackspaceStorage";
import SelectelStorage from "./storages/SelectelStorage";
import { getOptions } from "../../common-container/GetThirdPartyStoragesOptions";
class ThirdPartyStoragesModule extends React.PureComponent {
  constructor(props) {
    super(props);
    this.state = {
      comboBoxOptions: [],
      storagesInfo: {},

      selectedStorageTitle: "",
      selectedStorageId: "",
    };
  }
  componentDidMount() {
    const { onSetStorageId, thirdPartyStorage } = this.props;
    if (thirdPartyStorage) {
      const parameters = getOptions(thirdPartyStorage);

      const {
        comboBoxOptions,
        storagesInfo,
        selectedStorageTitle,
        selectedStorageId,
      } = parameters;

      onSetStorageId && onSetStorageId(selectedStorageId);

      this.setState({
        comboBoxOptions,
        storagesInfo,

        selectedStorageTitle,
        selectedStorageId,
      });
    }
  }
  onSelect = (option) => {
    const selectedStorageId = option.key;
    const { storagesInfo } = this.state;
    const { onSetStorageId } = this.props;
    const storage = storagesInfo[selectedStorageId];

    onSetStorageId && onSetStorageId(storage.id);

    this.setState({
      selectedStorageTitle: storage.title,
      selectedStorageId: storage.id,
    });
  };
  render() {
    const {
      comboBoxOptions,
      selectedStorageTitle,
      selectedStorageId,
      storagesInfo,
    } = this.state;
    const { thirdPartyStorage } = this.props;

    const commonProps = {
      selectedStorage: storagesInfo[selectedStorageId],
    };

    const { GoogleId, RackspaceId, SelectelId, AmazonId } = ThirdPartyStorages;

    return (
      <>
        <ComboBox
          options={comboBoxOptions}
          selectedOption={{ key: 0, label: selectedStorageTitle }}
          onSelect={this.onSelect}
          isDisabled={!!!thirdPartyStorage}
          noBorder={false}
          scaled
          scaledOptions
          dropDownMaxHeight={400}
          className="backup_combo"
        />

        {selectedStorageId === GoogleId && (
          <GoogleCloudStorage {...commonProps} {...this.props} />
        )}
        {selectedStorageId === RackspaceId && (
          <RackspaceStorage {...commonProps} {...this.props} />
        )}
        {selectedStorageId === SelectelId && (
          <SelectelStorage {...commonProps} {...this.props} />
        )}
        {selectedStorageId === AmazonId && (
          <AmazonStorage {...commonProps} {...this.props} />
        )}
      </>
    );
  }
}
export default inject(({ backup }) => {
  const { thirdPartyStorage } = backup;
  return {
    thirdPartyStorage,
  };
})(observer(ThirdPartyStoragesModule));
