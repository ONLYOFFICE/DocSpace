interface MainPanelProps {
  src: string;
  isLoading: boolean;

  isLastImage: boolean;
  isFistImage: boolean;

  setZoom: (scale: number) => void;
  onPrev: VoidFunction;
  onNext: VoidFunction;
}

export default MainPanelProps;
