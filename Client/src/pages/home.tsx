import Navbar from '../navbar';

export default function Home() {
    return (
        <>
            <Navbar />
            <div
                className="container d-flex justify-content-center align-items-center text-center fs-4 p-2 flex-column"
                style={{ height: '75vh' }}
            >
                <h1>
                    Effortlessly create movie polls and seamlessly integrate them with Discord, restricting polls to server members.
                </h1>
            </div>
        </>
    );
}
