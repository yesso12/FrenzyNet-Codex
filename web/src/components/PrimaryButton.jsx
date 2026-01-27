export default function PrimaryButton({ children, ...props }) {
  return (
    <button
      className="w-full bg-blue-600 text-white py-2 rounded hover:bg-blue-700"
      {...props}
    >
      {children}
    </button>
  )
}
